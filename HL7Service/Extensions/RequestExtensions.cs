namespace HL7Service.Extensions
{
    public static class RequestExtensions
    {
        public static async Task<string> ReadAsStringAsync(this Stream requestBody, bool leaveOpen = false)
        {
            String result = String.Empty;


            using (StreamReader reader = new StreamReader(requestBody, leaveOpen: leaveOpen))
            {
                result = await reader.ReadToEndAsync();
            }

            return result;
        }
    }
}
