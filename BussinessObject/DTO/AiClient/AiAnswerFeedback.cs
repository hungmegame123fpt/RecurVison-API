using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTO.AiClient
{
    public class AiAnswerFeedback
    {
        [JsonProperty("score")]
        public decimal Score { get; set; }

        [JsonProperty("feedback")]
        public string Feedback { get; set; } = string.Empty;

        [JsonProperty("suggestions")]
        public string? Suggestions { get; set; }
    }
}
