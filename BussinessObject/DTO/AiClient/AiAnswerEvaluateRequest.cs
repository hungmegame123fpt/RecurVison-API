using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTO.AiClient
{
    public class AiAnswerEvaluateRequest
    {
        [JsonProperty("session_id")]
        public string SessionId { get; set; } = string.Empty;

        [JsonProperty("cleaned_cv_text")]
        public string CleanedCvText { get; set; } = string.Empty;

        [JsonProperty("job_description")]
        public string JobDescription { get; set; } = string.Empty;

        [JsonProperty("previous_questions")]
        public List<string> PreviousQuestions { get; set; } = new();

        [JsonProperty("answer_text")]
        public string AnswerText { get; set; } = string.Empty;
    }
}
