
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Azure.Identity;
using HL7Service.Enums;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using Microsoft.Identity.Client;
using Microsoft.Extensions.Azure;
using HL7Service.Entities;

namespace HL7ServiceTest.v1
{
    public class HL7ControllerTest : IClassFixture<WebApplicationFactory<HL7Service.Program>>
    {
        private readonly WebApplicationFactory<HL7Service.Program> _factory;

        public HL7ControllerTest(WebApplicationFactory<HL7Service.Program> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task ParseControllerEndPointCheck()
        {
            // Arrange
            var client = _factory.CreateClient();
            var TenantId = @"2518dfa4-aca7-4d13-9094-58ff8149df3d";
            var ClientId = @"967717ea-155e-4fae-a552-193095172957";
            var ClientSecret = @"G0K8Q~3tyN.~nyKPXoXEK0FzexwDw-OsJiLgGb-l";

            try
            {
                // Acquire a authentication token against Microsoft Azure
                var authToken = await AzureTokenGenerator.GetFreshToken(TenantId, ClientId, ClientSecret);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

                // Act
                var currentWorkingDirectory = $"{Environment.CurrentDirectory}";
                var inputDataFilename = Path.Combine(currentWorkingDirectory, "Documentation\\OriginalInputData.csv");
                var originalInputData = File.ReadAllText(inputDataFilename);

                DataFormats outputDataFormat = DataFormats.BraceDelimited;

                var httpRequestContent = new StringContent(originalInputData, Encoding.UTF8, "text/plain");
                var response = await client.PostAsync($"api/v1/HL7/Parse?Delimiter={Delimiters.Comma}&TextQualifier={TextQualifiers.DoubleQuotes}&DataFormat={outputDataFormat}", httpRequestContent);

                // Assert
                response.EnsureSuccessStatusCode(); // Status Code 200-299

                var outputDataFilename = Path.Combine(currentWorkingDirectory, "Documentation\\OriginalOutputData.csv");
                var originalOutputData = File.ReadAllText(outputDataFilename);
                string expectedData = originalOutputData.Trim().ReplaceLineEndings();

                string requestBodyContent = await response.Content.ReadAsStringAsync();
                string actualData = requestBodyContent.Trim().ReplaceLineEndings();

                Assert.Equal(expectedData, actualData);

                if (outputDataFormat == DataFormats.BraceDelimited || outputDataFormat == DataFormats.CommaDelimited)
                {
                    Assert.Equal("text/plain; charset=utf-8", response.Content?.Headers?.ContentType?.ToString());
                }
                else if (outputDataFormat == DataFormats.Json)
                {
                    Assert.Equal("application/json; charset=utf-8", response.Content?.Headers?.ContentType?.ToString());
                }
                else if (outputDataFormat == DataFormats.Xml)
                {
                    Assert.Equal("application/xml; charset=utf-8", response.Content?.Headers?.ContentType?.ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
