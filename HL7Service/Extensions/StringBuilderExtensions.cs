using HL7Service.Enums;
using System.Text;
using static HL7Service.Entities.HL7Engine;

namespace HL7Service.Extensions
{
    public static class StringBuilderExtensions
    {
        /// <summary>
        ///     Test the beginning of the string for a starting TextQualifier, which signifies it is a string, not a number
        /// </summary>
        /// <param name="value"></param>
        /// <param name="tq"></param>
        /// <returns></returns>
        public static bool IsStringValue(this StringBuilder value, TextQualifiers tq)
        {
            return (value.ToString().StartsWith((char)tq));
        }

        /// <summary>
        ///     Remove the Starting and/or Ending TextQualifier passed as argument, if they exist
        /// </summary>
        /// <param name="value"></param>
        /// <param name="tq"></param>
        /// <returns></returns>
        public static string Trim(this StringBuilder value, TextQualifiers tq)
        {
            return value.ToString().TrimTextQualifier(tq);
        }

        /// <summary>
        ///     If a string representation of a numeric value, return a number.
        ///     If a string representation of a string value, return a string.
        ///     If the string length is 0, return an empty string regardless
        /// </summary>
        /// <param name="value"></param>
        /// <param name="textQualifierCount"></param>
        /// <returns></returns>
        public static object GetValue(this StringBuilder value, TextQualifiers tq)
        {
            object result = string.Empty;


            if (value.IsStringValue(tq))
            {
                // this is a string value, remove any TextQualifiers pre or post
                result = value.Trim(tq);
            }
            else
            {
                if (double.TryParse(value.ToString(), out double scalarValue))
                {
                    // this is a scalar value
                    result = scalarValue;
                }
            }

            return result;
        }
    }
}

