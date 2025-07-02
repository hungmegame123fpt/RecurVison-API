using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTO.AiClient
{
    public class AiQuestionData
    {
        [JsonProperty("id")]
        public string Id { get; set; } = string.Empty;

        [JsonProperty("label")]
        public string Label { get; set; } = string.Empty;

        [JsonProperty("type")]
        public string Type { get; set; } = "text";

        [JsonProperty("placeholder")]
        public string? Placeholder { get; set; }

        [JsonProperty("required")]
        public bool Required { get; set; }
    }
}
