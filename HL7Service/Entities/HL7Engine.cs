using HL7Service.Enums;
using HL7Service.Exceptions;
using HL7Service.Extensions;
using System.Data;
using System;
using System.Text;


namespace HL7Service.Entities
{
    public class HL7Engine
    {

#region "Properties"

        public Delimiters Delimiter
        {
            get;
            private set;
        }

        public TextQualifiers TextQualifier
        {
            get;
            private set;
        }

        #endregion "Properties"


#region "ctors"

        public HL7Engine(Delimiters delimiter, TextQualifiers textQualifier = TextQualifiers.DoubleQuotes)
        {
            this.Delimiter = delimiter;
            this.TextQualifier = textQualifier;
        }

#endregion "ctors"


#region "Public Methods"


        /// <summary>
        ///     Accepts multi-line serialized data string and returns list of strings, each element representing one row from string with new line chars
        ///     
        ///     NOTE: Using this method as the entry-point to processing will ensure the NewLineChar is normalized
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        /// <exception cref="EmptyBodyException"></exception>
        /// <exception cref="InvalidInputDataException"></exception>
        public string[] GetAllRows(string data)
        {
            string[] result = { };
            string normalizedLineEndings = data.ReplaceLineEndings();


            if (string.IsNullOrEmpty(data.Trim()))
            {
                throw new EmptyBodyException();
            }
            else
            {
                result = normalizedLineEndings.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

                if (result.Length < 2)
                {
                    throw new InvalidInputDataException();
                }
            }

            return result;
        }

        /// <summary>
        ///     Accepts multi-line serialized data and returns the first row of field labels
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public string GetHeaderRow(string data)
        {
            var rows = this.GetAllRows(data);

            string result = rows[0];

            return result;
        }

        /// <summary>
        ///     Accepts multi-line serialized data and returns list of individual rows of data
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public List<string> GetDataRows(string data)
        {
            var rows = this.GetAllRows(data);

            // return all but the first row (labels), all data rows as a list of strings
            List<string> result = rows.Skip(1).ToList();

            return result;
        }

        /// <summary>
        ///     Accepts serialized string of first row of field labels, returns a string list of labels
        /// </summary>
        /// <param name="headerRow"></param>
        /// <returns></returns>
        public List<string> GetHeaderListFromHeaderRow(string headerRow)
        {
            List<string> result = new List<string>();


            if (headerRow.Contains('\r') || headerRow.Contains('\n') || headerRow.Contains("\r\n"))
            {
                throw new ArgumentException($"The value '{headerRow}' cannot contain NewLineChar", nameof(headerRow));
            }
            else
            {
                if (headerRow.Contains((char)this.Delimiter) || headerRow.Contains('\n') || headerRow.Contains("\r\n"))
                {
                    string[] fields = headerRow.Split((char)this.Delimiter, StringSplitOptions.TrimEntries);

                    foreach (string field in fields)
                    {
                        result.Add(field.TrimTextQualifier(this.TextQualifier));
                    }
                }
                else
                {
                    throw new InvalidDelimiterException($"The HeaderRow '{headerRow}' does not contain the delimiter {this.Delimiter}");
                }
            }

            return result;
        }

        public List<EntityObject> GetEntityListFromDataRows(List<string> fieldLabels, List<string> dataRows)
        {
            List<EntityObject> result = new List<EntityObject>();

            
            // loop thru the data rows
            for (int dataRowIndex = 0; dataRowIndex < dataRows.Count; dataRowIndex++)
            {
                var dataRow = dataRows[dataRowIndex];
                var entityProperties = new List<EntityProperty>();

                if (this.FieldDelimiterExists(dataRow))
                {
                    object[] fieldValues = this.ParseDataRow(dataRow);

                    if (fieldLabels.Count == fieldValues.Length)
                    {
                        // loop thru the fields in the row
                        for (int fieldValueIndex = 0; fieldValueIndex < fieldValues.Length; fieldValueIndex++)
                        {
                            string fieldLabel = fieldLabels[fieldValueIndex];
                            object fieldValue = fieldValues[fieldValueIndex];

                            EntityProperty ep = new EntityProperty(fieldValueIndex, fieldLabel, fieldValue);

                            entityProperties.Add(ep);
                        }
                    }
                    else
                    {
                        throw new InconsistentFieldAndValueCountException(fieldLabels.Count, dataRows.Count, dataRowIndex + 2);
                    }
                }
                else
                {
                    // +2 because it is zero based and we need to skip over the headerRow
                    throw new InvalidDelimiterException($"The DataRow '{dataRow}' at line '{dataRowIndex + 2}' does not contain the delimiter {this.Delimiter}");
                }

                EntityObject eo = new EntityObject()
                {
                    Properties = entityProperties,
                };

                result.Add(eo);
            }

            return result;
        }

        public bool FieldDelimiterExists(string dataRow)
        {
            bool result = false;


            switch (this.Delimiter)
            {
                case Delimiters.Pipe:
                    result = dataRow.Contains('|');
                    break;

                case Delimiters.Comma:
                    result = dataRow.Contains(',');
                    break;

                case Delimiters.Semicolon:
                    result = dataRow.Contains(';');
                    break;

                case Delimiters.Tab:
                    result = dataRow.Contains('\t');
                    break;
            }

            return result;
        }

        /// <summary>
        ///     Go thru each character and apply it to a field
        ///     The decision to start a new field depends on the delimiter and whether it is a number or not (inside a textqualifier)
        /// </summary>
        /// <param name="dataRow"></param>
        /// <returns></returns>
        public object[] ParseDataRow(string dataRow)
        {
            List<object> result = new List<object>();
    
            StringBuilder value = new StringBuilder();

            // even number terminates the current field, odd numbers are starting it
            int textQualifierCount = 0;
            int numericQualifierCount = 0;


            // walk the length of the string and parse out into field value list, based on TextQualifiers and Delimiters
            for (int i = 0; i < dataRow.Length; i++)
            {
#if DEBUG
                // this is only here for debugging purposes, it will be omitted in release builds
                string s = dataRow[i].ToString();
                if (i == 29)
                { 
                    Console.WriteLine(s); 
                }
#endif
                char c = dataRow[i];

                if (c == (char)this.TextQualifier)
                {
                    // toggle the building of a string value on or off
                    textQualifierCount++;
                }
                else if (textQualifierCount.RepresentsEndOfValue() && numericQualifierCount.RepresentsEndOfValue() && (c != (char)this.Delimiter))
                {
                    // YES, we ARE NOT in the middle of a string value and we hit a field delimiter, so it must be a numeric value
                    numericQualifierCount++;
                }
                else if (textQualifierCount.RepresentsEndOfValue() && numericQualifierCount.RepresentsStartOfValue() && (c == (char)this.Delimiter))
                {
                    // YES, we ARE in the middle of a string value and we hit a field delimiter, so it must be the end of numeric value
                    numericQualifierCount++;
                }

                if (numericQualifierCount.RepresentsStartOfValue())
                {
                    // YES, we are in the middle of a numeric value
                    value.Append(c);
                }
                else if (textQualifierCount.RepresentsStartOfValue())
                {
                    // YES, we are in the middle of a string
                    value.Append(c);
                }
                else if ((textQualifierCount.RepresentsEndOfValue() || numericQualifierCount.RepresentsEndOfValue()) && i > 0)
                {
                    // YES, we received the closing string character
                    result.Add(value.GetValue(this.TextQualifier));

                    if (value.IsStringValue(this.TextQualifier))
                    {
                        // increment the dataRow pointer by one to prevent the adding of an errant empty element
                        i++;
                    }

                    // reset the value for the next field
                    value.Length = 0;
                }
            }

            if (value.Length > 0)
            {
                // there was a numeric value at the end of the line, so we didn't see a termination char
                result.Add(value.GetValue(this.TextQualifier));
            }

            return result.ToArray();
        }

#endregion "Methods"

       
    }
}
