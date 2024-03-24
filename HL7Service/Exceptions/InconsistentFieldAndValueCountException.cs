namespace HL7Service.Exceptions
{
    public class InconsistentFieldAndValueCountException : Exception
    {
        public InconsistentFieldAndValueCountException(int fieldCount, int valueCount, int lineNumber) : base($"InconsistentFieldAndValueCount Error: In line '{lineNumber}', the number of field labels '{fieldCount}' does not match the number of field values '{valueCount}'.")
        {
            
        }
    }
}
