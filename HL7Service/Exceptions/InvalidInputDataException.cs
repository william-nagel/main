namespace HL7Service.Exceptions
{
    public class InvalidInputDataException : Exception
    {
        public InvalidInputDataException() : base("The input data does not contain at least one row of field labels and one row of field values.")
        {
            
        }
    }
}
