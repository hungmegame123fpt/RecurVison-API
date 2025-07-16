using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BusinessObject.DTO.AiClient
{
    public class AiAnswerEvaluateResponse
    {
        [JsonPropertyName("error_code")]
        public int ErrorCode { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; } = string.Empty;

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("data")]
        public AiAnswerData Data { get; set; } = new();
    }
}
