
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace HisaabApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AiAssistantController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;

        public AiAssistantController(
            IConfiguration configuration,
            IHttpClientFactory httpClientFactory)
        {
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
        }

        public record ChatRequest(string Message);

        [HttpPost("chat")]
        public async Task<IActionResult> Chat(
            [FromBody] ChatRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Message))
            {
                return BadRequest(new
                {
                    error = "Please enter a message."
                });
            }

            var apiKey = _configuration["Gemini:ApiKey"];

            if (string.IsNullOrWhiteSpace(apiKey))
            {
                return StatusCode(500, new
                {
                    error = "Gemini API key is not configured."
                });
            }

            try
            {
                var client = _httpClientFactory.CreateClient();

                var url =
                    "https://generativelanguage.googleapis.com/v1beta/" +
                   "models/gemini-3.8-flash:generateContent";

                using var httpRequest = new HttpRequestMessage(
                    HttpMethod.Post, url);

                httpRequest.Headers.Add("x-goog-api-key", apiKey);

                httpRequest.Content = JsonContent.Create(new
                {
                    system_instruction = new
                    {
                        parts = new[]
                        {
                            new
                            {
                                text =
                                    "You are Hisaab AI, a helpful personal " +
                                    "budgeting assistant for Pakistan. " +
                                    "Give practical saving and expense " +
                                    "management advice. Use PKR for money. " +
                                    "Be friendly and concise. Do not claim " +
                                    "to know financial data the user has " +
                                    "not provided."
                            }
                        }
                    },
                    contents = new[]
                    {
                        new
                        {
                            parts = new[]
                            {
                                new { text = request.Message }
                            }
                        }
                    }
                });

                using var response = await client.SendAsync(httpRequest);
                var responseBody =
                    await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine(
                        $"Gemini API error ({(int)response.StatusCode}): " +
                        responseBody);

                    return StatusCode(502, new
                    {
                        error = "Gemini request failed.",
                        statusCode = (int)response.StatusCode,
                        details = responseBody
                    });
                }

                using var document = JsonDocument.Parse(responseBody);

                var reply = document.RootElement
                    .GetProperty("candidates")[0]
                    .GetProperty("content")
                    .GetProperty("parts")[0]
                    .GetProperty("text")
                    .GetString();

                return Ok(new
                {
                    reply = reply ?? "Please try asking again."
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Gemini error: {ex}");

                return StatusCode(502, new
                {
                    error = "Could not get a response from Gemini.",
                    details = ex.Message
                });
            }
        }
    }
}
