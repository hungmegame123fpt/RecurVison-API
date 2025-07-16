using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BusinessObject.DTO.AiClient
{
    public class AiAnswerData
    {
        [JsonPropertyName("feedback")]
        public AiAnswerFeedback Feedback { get; set; } = new();

        [JsonPropertyName("next_question")]
        public AiQuestion? NextQuestion { get; set; }

        [JsonPropertyName("answer")]
        public string? Answer { get; set; }

        [JsonPropertyName("current_iteration")]
        public int CurrentIteration { get; set; }

        [JsonPropertyName("completeness_score")]
        public decimal CompletenessScore { get; set; }

        [JsonPropertyName("should_continue")]
        public bool ShouldContinue { get; set; }
    }
}
