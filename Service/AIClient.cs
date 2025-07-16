using BusinessObject.DTO;
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
        public AIClient(HttpClient httpClient, ILogger<AIClient> logger, IUnitOfWork unitOfWork)
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
            try
            {
                var json = JsonConvert.SerializeObject(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // Create the request
                var httpRequest = new HttpRequestMessage(HttpMethod.Post, "https://recruvision-h8freyfdh3bsb9fs.southeastasia-01.azurewebsites.net/api/v1/question-composer/answer")
                {
                    Content = content
                };

                httpRequest.Headers.Add("lang", "vi");
                httpRequest.Headers.Add("accept", "application/json");

                var response = await _httpClient.SendAsync(httpRequest);
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("AI answer evaluation failed: {StatusCode} - {Error}", response.StatusCode, errorContent);
                    throw new Exception($"AI error: {response.StatusCode} - {errorContent}");
                }

                var result = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<AiAnswerEvaluateResponse>(result, _jsonOptions)!;
            }
            catch(Exception ex) {
                _logger.LogError(ex, "Error calling AI /question-composer/answer");
                throw;
            }
        }
        public async Task<AiJobMatchingData> GetSuggestedJobsForCvAsync(int cvId)
        {
            var analysis = await _unitOfWork.CvAnalysisResult.GetLatestAnalysisForCvAsync(cvId);
            if (analysis == null)
                throw new InvalidOperationException($"No analysis found for CvId: {cvId}");
            var projectDtos = new List<object>();

            foreach (var p in analysis.Projects)
            {
                var techStacks = await _unitOfWork.CvProjectTechStack.FindAsync(t => t.CvProjectId == p.Id);
                var techNames = techStacks.Select(t => t.TechName).ToList();

                projectDtos.Add(new
                {
                    p.Title,
                    p.Description,
                    tech_stack = techNames
                });
            }
            var requestBody = new
            {
                jd_alignment = analysis.JdAlignment,
                cv_analysis_result = new
                {
                    name = analysis.Name,
                    email = analysis.Email,
                    phone = analysis.Phone,
                    summary = analysis.Summary,
                    skills = analysis.Skills.Select(s => s.SkillName).ToList(),
                    education = analysis.Education.Select(e => new
                    {
                        e.Degree,
                        e.Institution,
                        e.StartYear,
                        e.EndYear,
                        e.Description
                    }),
                    projects = projectDtos,
                    certifications = analysis.Certifications.Select(c => new
                    {
                        c.Name,
                        c.Issuer,
                        c.TimePeriod,
                        c.Description
                    })
                }
            };

            var httpRequest = new HttpRequestMessage(HttpMethod.Post,
                "https://recruvision-h8freyfdh3bsb9fs.southeastasia-01.azurewebsites.net/api/v1/job-matching/suggest")
            {
                Content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json")
            };

            httpRequest.Headers.Add("lang", "vi");
            httpRequest.Headers.Add("accept", "application/json");

            var response = await _httpClient.SendAsync(httpRequest);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var aiResponse = JsonSerializer.Deserialize<AiJobMatchingResponse>(json, _jsonOptions);
            return new AiJobMatchingData
            {
                SuggestedJobs = aiResponse?.Data.SuggestedJobs ?? new(),
                SuggestedCourses = aiResponse?.Data.SuggestedCourses ?? new()
            };
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
		public AiCvAnalysisResult CvAnalysisResult { get; set; } = null!;

		[JsonPropertyName("jd_alignment")]
		public string JdAlignment { get; set; } = null!;
	}
    public class AiJobMatchingData
    {
        [JsonPropertyName("suggested_jobs")]
        public List<SuggestedJob> SuggestedJobs { get; set; } = new();
        [JsonPropertyName("suggested_courses")]
        public List<SuggestedCourse> SuggestedCourses { get; set; } = new();
    }

    public class AiJobMatchingResponse
    {
        [JsonPropertyName("data")]
        public AiJobMatchingData Data { get; set; } = new();
    }
    public class SuggestedCourse
    {
        [JsonPropertyName("course_name")]
        public string CourseName { get; set; } = string.Empty;

        [JsonPropertyName("platform")]
        public string Platform { get; set; } = string.Empty;

        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;

        [JsonPropertyName("estimated_duration")]
        public string EstimatedDuration { get; set; } = string.Empty;

        [JsonPropertyName("url")]
        public string Url { get; set; } = string.Empty;
    }

}
