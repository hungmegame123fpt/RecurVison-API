using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTO.AiClient
{
    public class AiQuestion
    {
        [JsonProperty("id")]
        public string Id { get; set; } = string.Empty;

        [JsonProperty("Question")]
        public string Question { get; set; } = string.Empty;

        [JsonProperty("Question_type")]
        public string QuestionType { get; set; } = "text_input";

        [JsonProperty("subtitle")]
        public string? Subtitle { get; set; }

        [JsonProperty("Question_data")]
        public List<AiQuestionData>? QuestionData { get; set; }
    }
}
