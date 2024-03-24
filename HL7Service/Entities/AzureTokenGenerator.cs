using Microsoft.Graph.ExternalConnectors;
using Nancy.Json;
using System.Text.Json;


namespace HL7Service.Entities
{
    public class AzureTokenGenerator
    {

        public static async Task<String> GetFreshToken(string tenantId, string clientId, string clientSecret)
        {
            var tokenUrl = $"https://login.microsoftonline.com/{tenantId}/oauth2/v2.0/token";


            using (var client = new HttpClient())
            {
                var content = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    ["grant_type"] = "client_credentials",
                    ["client_id"] = clientId,
                    ["client_secret"] = clientSecret,
                    ["scope"] = "https://graph.microsoft.com/.default"
                });

                var response = await client.PostAsync(tokenUrl, content);
                var jsonResponse = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    throw new ApplicationException($"Failed to retrieve access token: {jsonResponse}");
                }

                var rsps = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonResponse);

                if (rsps != null && rsps.TryGetValue("access_token", out var accessToken))
                {
                    return accessToken.ToString()!;
                }
                    
                throw new ApplicationException("Access token not found in the response.");
            }
        }
    }
}
