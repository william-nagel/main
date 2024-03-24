using HL7Service.Entities;
using static HL7Service.Entities.HL7Engine;

namespace HL7Service.Exceptions
{
    public class InvalidDelimiterException : Exception
    {
        public InvalidDelimiterException(string message) : base(message)
        {

        }
    }
}
