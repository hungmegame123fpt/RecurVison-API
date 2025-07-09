using BusinessObject.DTO.AiClient;
using BusinessObject.DTO.CV;
using BusinessObject.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Repository.Interface;
using Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Service
{
    public class AIClient : IAIClient
    {
        private readonly HttpClient _httpClient;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<AIClient> _logger;
		private readonly JsonSerializerOptions _jsonOptions = new()
		{
			PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
			PropertyNameCaseInsensitive = true
		};
		public AIClient(HttpClient httpClient, ILogger<AIClient> logger, IUnitOfWork unitOfWork )
        {
            _httpClient = httpClient;
            _logger = logger;
            _unitOfWork = unitOfWork;
        }
        public async Task<AiCvAnalysisResponse> AnalyzeCvAsync(int Cvid, IFormFile jdFile)
        {
			using var content = new MultipartFormDataContent();
            var cv = await _unitOfWork.CVRepository.GetByIdAsync(Cvid);

			// 1. Add cv_file_url (string)
			content.Add(new StringContent(cv.FilePath), "cv_file_url");

			// 2. Add jd_file (binary file)
			var fileStreamContent = new StreamContent(jdFile.OpenReadStream());
			fileStreamContent.Headers.ContentType = new MediaTypeHeaderValue(jdFile.ContentType);
			content.Add(fileStreamContent, "jd_file", jdFile.FileName);
			var request = new HttpRequestMessage(HttpMethod.Post, "https://recruvision-h8freyfdh3bsb9fs.southeastasia-01.azurewebsites.net/api/v1/cv/process-url")
			{
				Content = content
			};

			// 4. Add headers
			request.Headers.Add("checksum", "4pI2ZAxB7X8N9sM5R8k_AfF4PLbJnvYsV2gJJei8BjI=");
			request.Headers.Add("lang", "vi");

			var response = await _httpClient.SendAsync(request);
			response.EnsureSuccessStatusCode();

			var json = await response.Content.ReadAsStringAsync();
			var result = JsonSerializer.Deserialize<AiCvAnalysisResponse>(json, _jsonOptions)!;
            return result;
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
        public async Task<AiAnswerEvaluateResponse> EvaluateAnswerAsync(AiAnswerEvaluateRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync("https://recruvision-h8freyfdh3bsb9fs.southeastasia-01.azurewebsites.net/api/v1/question-composer/answer", request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<AiAnswerEvaluateResponse>();
        }
    }
	public class AiCvAnalysisResponse
	{
		[JsonPropertyName("data")]
		public AiCvAnalysisData Data { get; set; } = null!;
	}

	public class AiCvAnalysisData
	{
		[JsonPropertyName("cv_analysis_result")]
		public CvAnalysisResult CvAnalysisResult { get; set; } = null!;

		[JsonPropertyName("jd_alignment")]
		public string JdAlignment { get; set; } = null!;
	}
}
