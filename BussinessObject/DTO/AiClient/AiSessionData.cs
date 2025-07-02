using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTO.AiClient
{
    public class AiSessionData
    {
        [JsonProperty("session_id")]
        public string SessionId { get; set; } = string.Empty;

        [JsonProperty("questions")]
        public List<AiQuestion> Questions { get; set; } = new();

        [JsonProperty("analysis")]
        public string? Analysis { get; set; }

        [JsonProperty("next_focus_areas")]
        public List<string>? NextFocusAreas { get; set; }
    }
}
