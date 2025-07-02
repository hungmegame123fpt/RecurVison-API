using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTO.AiClient
{
    public class AiAnswerData
    {
        [JsonProperty("feedback")]
        public AiAnswerFeedback Feedback { get; set; } = new();

        [JsonProperty("next_question")]
        public AiQuestion? NextQuestion { get; set; }

        [JsonProperty("answer")]
        public string? Answer { get; set; }

        [JsonProperty("current_iteration")]
        public int CurrentIteration { get; set; }

        [JsonProperty("completeness_score")]
        public decimal CompletenessScore { get; set; }

        [JsonProperty("should_continue")]
        public bool ShouldContinue { get; set; }
    }
}
