using HL7Service.Enums;
using System.Text;
using static HL7Service.Entities.HL7Engine;

namespace HL7Service.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        ///     Strip the string text qualifier from the start or end of a string, if they exists
        /// </summary>
        /// <param name="value"></param>
        /// <param name="tq"></param>
        /// <returns></returns>
        public static string TrimTextQualifier(this string value, TextQualifiers tq)
        {
            String result = value;

            if (tq != TextQualifiers.None) 
            {
                char lookFor = (tq == TextQualifiers.SingleQuotes ? '\'' : '\"');

                if (result.StartsWith(lookFor))
                {
                    result = result.Substring(1);
                }

                if (result.EndsWith(lookFor))
                {
                    result = result.Substring(0, result.Length - 1);
                }
            }

            return result;
        }
    }
}

