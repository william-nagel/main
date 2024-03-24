using Azure;
using HL7Service.Entities;
using HL7Service.Enums;
using HL7Service.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace HL7Service.Controllers.v1
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [Authorize]
    public class HL7Controller : ControllerBase
    {
        private readonly ILogger<HL7Controller> _logger;

        public HL7Controller(ILogger<HL7Controller> logger)
        {
            _logger = logger;
        }

 
        [HttpPost("Parse")]
        public async Task<IActionResult> ParseMethod()
        {
            string identity = $"{User.Identity?.Name}";
            string requestBody = await this.Request.Body.ReadAsStringAsync();

            try
            {
                if (requestBody.IsNullOrEmpty())
                {
                    throw new ArgumentNullException("Request.Body", "Cannot be NULL");
                }
                else
                {
                    _logger.LogDebug($"Identity: {identity}; Body: {requestBody}");

                    // 1. collect service arguments
                    string delimiterArg = Request.Query["Delimiter"]; 
                    string textQualifierArg = Request.Query["TextQualifier"];
                    string outputFormatArg = Request.Query["DataFormat"];

                    // 2. transform arguments into enums
                    Delimiters delimiter = (Delimiters)Enum.Parse(typeof(Delimiters), delimiterArg, true);
                    TextQualifiers textQualifier = (TextQualifiers)Enum.Parse(typeof(TextQualifiers), textQualifierArg, true);
                    DataFormats dataFormat = (DataFormats)Enum.Parse(typeof(DataFormats), outputFormatArg, true);

                    // 3. generate instance of parsing engine
                    HL7Engine parser = new HL7Engine(delimiter, textQualifier);

                    // 4. generate a list of header labels
                    string headerRow = parser.GetHeaderRow(requestBody);
                    List<string> headerList = parser.GetHeaderListFromHeaderRow(headerRow);

                    // 5. generate a list of data rows
                    List<string> dataRows = parser.GetDataRows(requestBody);

                    // 6. generate a list of entities, containing key-value pairs
                    List<EntityObject> entities = parser.GetEntityListFromDataRows(headerList, dataRows);

                    // 7. serialize output and send Response to client
                    String responseBody = string.Empty;

                    // 8. Format outgoing data according to client requested format
                    if (dataFormat == DataFormats.BraceDelimited) { responseBody = EntitySerializer.ToBraceSeparatedString(entities); }
                    else if (dataFormat == DataFormats.CommaDelimited) { responseBody = EntitySerializer.ToCommaSeparatedString(entities); }
                    else if (dataFormat == DataFormats.Json) { responseBody = EntitySerializer.ToJsonString(entities); }
                    else if (dataFormat == DataFormats.Xml) { responseBody = EntitySerializer.ToXmlString(entities); }

                    // 9. Convert string output to byte array
                    var data = Encoding.UTF8.GetBytes(responseBody);

                    // 10. Set the content type to plain text, so client knows how to treat the response
                    Response.ContentType = "text/plain; charset=utf-8";

                    if (dataFormat == DataFormats.Json) { Response.ContentType = "application/json; charset=utf-8"; }
                    else if (dataFormat == DataFormats.Xml) { Response.ContentType = "application/xml; charset=utf-8"; }

                    // 11. Writing to the response body
                    await Response.Body.WriteAsync(data, 0, data.Length);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error: {ex.Message}; Identity: {identity}; Body: {requestBody}; Url: {Request.GetDisplayUrl()}");
            }

            // Since we're manually writing to the response, we do not need to return a typical action result.
            // We return an empty status code to indicate to ASP.NET Core's pipeline that we've handled the response.
            return new EmptyResult();
        }
    }
}
