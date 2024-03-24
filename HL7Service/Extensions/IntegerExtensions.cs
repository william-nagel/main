using HL7Service.Enums;
using System.Text;
using static HL7Service.Entities.HL7Engine;

namespace HL7Service.Extensions
{
    public static class IntegerExtensions
    {
        /// <summary>
        ///     Odd count means we have seen the beginning of a string value
        /// </summary>
        /// <param name="textQualifierCount"></param>
        /// <returns></returns>
        public static bool RepresentsStartOfValue(this int textQualifierCount) 
        { 
            return (textQualifierCount % 2 == 1);
        }

        /// <summary>
        ///     Even counts means we have seen the ending of a string value
        /// </summary>
        /// <param name="textQualifierCount"></param>
        /// <returns></returns>
        public static bool RepresentsEndOfValue(this int textQualifierCount)
        {
            return (textQualifierCount % 2 == 0);
        }
    }
}

