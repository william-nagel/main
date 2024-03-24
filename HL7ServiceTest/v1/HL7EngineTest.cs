using HL7Service.Entities;
using HL7Service.Enums;
using HL7Service.Exceptions;
using System.Reflection.PortableExecutable;
using static HL7Service.Entities.HL7Engine;


namespace HL7ServiceTest.v1
{
    public class HL7EngineTest
    {
        // TODO: finish adding use cases for other TextQualifiers, if needed

        /// <summary>
        ///     Test unhappy path, looking for empty request body 
        /// </summary>
        [Theory]
        [InlineData(Delimiters.Comma, "")]
        [InlineData(Delimiters.Comma, " ")]
        public void ParseEmptyBodyCheck(Delimiters delimiter, string data)
        {
            var parser = new HL7Engine(delimiter);

            _ = Assert.Throws<EmptyBodyException>(() => parser.GetHeaderRow(data));
        }

        /// <summary>
        ///     Test happy path, testing for correct field Delimiter
        /// </summary>
        [Theory]
        [InlineData(Delimiters.Comma, "\"Patient Name\",\"SSN\",\"Age\",\"Phone Number\",\"Status\"\n\"Prescott, Zeke\",\"542-51-6641\",21,\"801-555-2134\",\"Opratory=2,PCP=1\"\n")]
        [InlineData(Delimiters.Pipe, "\"Patient Name\"|\"SSN\"|\"Age\"|\"Phone Number\"|\"Status\"\n\"Prescott, Zeke\"|\"542-51-6641\"|21|\"801-555-2134\"|\"Opratory=2,PCP=1\"\n")]
        [InlineData(Delimiters.Semicolon, "\"Patient Name\";\"SSN\";\"Age\";\"Phone Number\";\"Status\"\n\"Prescott, Zeke\";\"542-51-6641\";21;\"801-555-2134\";\"Opratory=2,PCP=1\"\n")]
        [InlineData(Delimiters.Tab, "\"Patient Name\"\t\"SSN\"\t\"Age\"\t\"Phone Number\"\t\"Status\"\n\"Prescott, Zeke\"\t\"542-51-6641\"\t21\t\"801-555-2134\"\t\"Opratory=2,PCP=1\"\n")]
        public void ParseValidDelimiterCheck(Delimiters delimiter, string data)
        {
            var parser = new HL7Engine(delimiter);

            var headerRow = parser.GetHeaderRow(data);

            Assert.NotNull(headerRow);
            Assert.NotEmpty(headerRow);

            var headers = parser.GetHeaderListFromHeaderRow(headerRow);

            Assert.True(headers.Count == 5);
        }

        /// <summary>
        ///     Test unhappy path, testing for incorrect field Delimiter
        /// </summary>
        [Theory]
        [InlineData(Delimiters.Comma, "\"Patient Name\"|\"SSN\"|\"Age\"|\"Phone Number\"|\"Status\"\n\"Prescott, Zeke\"|\"542-51-6641\"|21|\"801-555-2134\"|\"Opratory=2,PCP=1\"\n")]
        [InlineData(Delimiters.Pipe, "\"Patient Name\";\"SSN\";\"Age\";\"Phone Number\";\"Status\"\n\"Prescott, Zeke\";\"542-51-6641\";21;\"801-555-2134\";\"Opratory=2,PCP=1\"\n")]
        [InlineData(Delimiters.Semicolon, "\"Patient Name\"\t\"SSN\"\t\"Age\"\t\"Phone Number\"\t\"Status\"\n\"Prescott, Zeke\"\t\"542-51-6641\"\t21\t\"801-555-2134\"\t\"Opratory=2,PCP=1\"\n")]
        [InlineData(Delimiters.Tab, "\"Patient Name\",\"SSN\",\"Age\",\"Phone Number\",\"Status\"\n\"Prescott, Zeke\",\"542-51-6641\",21,\"801-555-2134\",\"Opratory=2,PCP=1\"\n")]
        public void ParseInvalidDelimiterCheck(Delimiters delimiter, string data)
        {
            var parser = new HL7Engine(delimiter);

            var headerRow = parser.GetHeaderRow(data);

            Assert.NotNull(headerRow);
            Assert.NotEmpty(headerRow);

            _ = Assert.Throws<InvalidDelimiterException>(() => parser.GetHeaderListFromHeaderRow(headerRow));
        }

        /// <summary>
        ///     Test happy path, looking for correct return of the header row
        /// </summary>
        [Theory]
        [InlineData(Delimiters.Comma, "\"Patient Name\",\"SSN\",\"Age\",\"Phone Number\",\"Status\"\n\"Prescott, Zeke\",\"542-51-6641\",21,\"801-555-2134\",\"Opratory=2,PCP=1\"\n\"Goldstein, Bucky\",\"635-45-1254\",42,\"435-555-1541\",\"Opratory=1,PCP=1\"\n\"Vox, Bono\",\"414-45-1475\",51,\"801-555-2100\",\"Opratory=3,PCP=2\"")]
        public void ParseGetHeaderRowCheck(Delimiters delimiter, string data)
        {
            var parser = new HL7Engine(delimiter);

            var headerRow = parser.GetHeaderRow(data);

            Assert.Equal("\"Patient Name\",\"SSN\",\"Age\",\"Phone Number\",\"Status\"", headerRow);
        }

        /// <summary>
        ///     Test happy path, looking for correct number of data rows returned and the correct elements
        /// </summary>
        [Theory]
        [InlineData(Delimiters.Comma, "\"Patient Name\",\"SSN\",\"Age\",\"Phone Number\",\"Status\"\n\"Prescott, Zeke\",\"542-51-6641\",21,\"801-555-2134\",\"Opratory=2,PCP=1\"\n\"Goldstein, Bucky\",\"635-45-1254\",42,\"435-555-1541\",\"Opratory=1,PCP=1\"\n\"Vox, Bono\",\"414-45-1475\",51,\"801-555-2100\",\"Opratory=3,PCP=2\"")]
        public void ParseGetDataRowsCheck(Delimiters delimiter, string data)
        {
            var parser = new HL7Engine(delimiter);

            var dataRows = parser.GetDataRows(data);
            int i = 0;

            Assert.True(dataRows.Count == 3);
            Assert.StartsWith("\"Prescott", dataRows[i++]);
            Assert.StartsWith("\"Goldstein", dataRows[i++]);
            Assert.StartsWith("\"Vox", dataRows[i++]);
        }


        /// <summary>
        ///     Test the most complicated parsing logic, why?? There are two different scenarios that trigger a new field value:
        ///     1. It can be a string containing the field delimiter.
        ///     2. Sometimes it is a number, which won't have text qualifiers at all.
        /// </summary>
        [Theory]
        // leading field is a string
        [InlineData(Delimiters.Comma, "\"Prescott, Zeke\",\"542-51-6641\",21,\"801-555-2134\",\"Opratory=2,PCP=1\"\n")]
        // leading field is a number
        [InlineData(Delimiters.Comma, "101,\"Prescott, Zeke\",\"542-51-6641\",21,\"Opratory=2,PCP=1\"\n")]
        // adding an empty string and an empty number field
        [InlineData(Delimiters.Comma, "\"Goldstein, Bucky\",\"\",,\"435-555-1541\",\"Opratory=1,PCP=1\"\n")]
        public void ParseParseDataRowCheck(Delimiters delimiter, string data)
        {
            var parser = new HL7Engine(delimiter);

            var dataList = parser.ParseDataRow(data);

            Assert.True(dataList.Length == 5);
        }

        /// <summary>
        ///     Test for a consistent number of entries between header and data rows
        /// </summary>
        [Theory]
        // two field test
        [InlineData(Delimiters.Comma, "\"Patient Name\",\"Age\"\n\"Goldstein, Bucky\",42")]
        // three field test
        [InlineData(Delimiters.Comma, "\"Patient Name\",\"SSN\",\"Age\"\n\"Prescott, Zeke\",\"542-51-6641\",21\n")]
        // four field test
        [InlineData(Delimiters.Comma, "\"Patient Name\",\"SSN\",\"Age\",\"Phone Number\"\n\"Goldstein, Bucky\",\"635-45-1254\",42,\"435-555-1541\"\n")]
        // five field test
        [InlineData(Delimiters.Comma, "\"Patient Name\",\"SSN\",\"Age\",\"Phone Number\",\"Status\"\n\"Prescott, Zeke\",\"542-51-6641\",21,\"801-555-2134\",\"Opratory=2,PCP=1\"\n")]
        public void ParseValidHeaderAndDataRowFieldCountCheck(Delimiters delimiter, string data)
        {
            var parser = new HL7Engine(delimiter);

            var headerRow = parser.GetHeaderRow(data);
            var headerList = parser.GetHeaderListFromHeaderRow(headerRow);

            var dataRows = parser.GetDataRows(data);
            var entityList = parser.GetEntityListFromDataRows(headerList, dataRows);

            // the total number of fields
            Assert.True(headerList.Count == entityList[0].Properties.Count);
        }

        /// <summary>
        ///     Test for an inconsistent number of field names and field values
        /// </summary>
        [Theory]
        // two field labels and three field values
        [InlineData(Delimiters.Comma, "\"Patient Name\",\"SSN\"\n\"Prescott, Zeke\",\"542-51-6641\",21")]
        // three field labels and four field values
        [InlineData(Delimiters.Comma, "\"Patient Name\",\"SSN\",\"Age\"\n\"Prescott, Zeke\",\"542-51-6641\",21,\"801-555-2134\"")]
        // four field labels and five field values
        [InlineData(Delimiters.Comma, "\"Patient Name\",\"SSN\",\"Age\",\"Phone Number\"\n\"Prescott, Zeke\",\"542-51-6641\",21,\"801-555-2134\",\"Opratory=2,PCP=1\"")]
        // five field labels and four field values
        [InlineData(Delimiters.Comma, "\"Patient Name\",\"SSN\",\"Age\",\"Phone Number\",\"Status\"\n\"Prescott, Zeke\",\"542-51-6641\",21,\"801-555-2134\"\n")]
        public void ParseInvalidHeaderAndDataRowFieldCountCheck(Delimiters delimiter, string data)
        {
            var parser = new HL7Engine(delimiter);

            var headerRow = parser.GetHeaderRow(data);
            var headerList = parser.GetHeaderListFromHeaderRow(headerRow);

            var dataRows = parser.GetDataRows(data);

            _ = Assert.Throws<InconsistentFieldAndValueCountException>(() => parser.GetEntityListFromDataRows(headerList, dataRows));
        }


        /// <summary>
        ///     This is the happy path, expecting proper parsing of the data
        /// </summary>
        [Theory]
        // leading field is a string
        [InlineData(Delimiters.Comma, "\"Patient Name\",\"SSN\",\"Age\",\"Phone Number\",\"Status\"\n\"Prescott, Zeke\",\"542-51-6641\",21,\"801-555-2134\",\"Opratory=2,PCP=1\"\n\"Goldstein, Bucky\",\"635-45-1254\",42,\"435-555-1541\",\"Opratory=1,PCP=1\"\n\"Vox, Bono\",\"414-45-1475\",51,\"801-555-2100\",\"Opratory=3,PCP=2\"")]
        // leading field is a number
        [InlineData(Delimiters.Comma, "\"RecordNumber\",\"Patient Name\",\"SSN\",\"Age\",\"Phone Number\",\"Status\"\n,101,\"Prescott, Zeke\",\"542-51-6641\",21,\"801-555-2134\",\"Opratory=2,PCP=1\"\n,102,\"Goldstein, Bucky\",\"635-45-1254\",42,\"435-555-1541\",\"Opratory=1,PCP=1\"\n,103,\"Vox, Bono\",\"414-45-1475\",51,\"801-555-2100\",\"Opratory=3,PCP=2\"")]
        public void ParseValidCheck(Delimiters delimiter, string data)
        {
            var parser = new HL7Engine(delimiter);

            var headerRow = parser.GetHeaderRow(data);
            var headerList = parser.GetHeaderListFromHeaderRow(headerRow);

            var dataRows = parser.GetDataRows(data);
            var entityList = parser.GetEntityListFromDataRows(headerList, dataRows);

            // the total number of fields
            Assert.True(headerList.Count == entityList[0].Properties.Count);
        }
    }
}