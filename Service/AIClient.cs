using BusinessObject.DTO.AiClient;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class AIClient : IAIClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<AIClient> _logger;

        public AIClient(HttpClient httpClient, ILogger<AIClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<AiSessionResponse?> StartSessionAsync(AiSessionRequest request)
        {
            var url = "https://recruvision-h8freyfdh3bsb9fs.southeastasia-01.azurewebsites.net/api/v1/question-composer/start-session";

            try
            {
                var json = JsonConvert.SerializeObject(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var httpRequest = new HttpRequestMessage(HttpMethod.Post, url)
                {
                    Content = content
                };
                httpRequest.Headers.Add("accept", "application/json");
                httpRequest.Headers.Add("lang", "vi");

                var response = await _httpClient.SendAsync(httpRequest);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("AI returned error: {StatusCode} - {Content}", response.StatusCode, await response.Content.ReadAsStringAsync());
                    return null;
                }

                var result = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<AiSessionResponse>(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling AI session API");
                return null;
            }
        }
    }
}
