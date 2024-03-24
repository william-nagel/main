namespace HL7Service.Exceptions
{
    public class EmptyBodyException : Exception
    {
        public EmptyBodyException() : base("Request.Body cannot be empty or null.")
        {
            
        }
    }
}
