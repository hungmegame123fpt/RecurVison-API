using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTO.AiClient
{
    public class SubmitAnswerRequest
    {
        [JsonProperty("session_id")]
        public string SessionId { get; set; }

        [JsonProperty("answer_text")]
        public string AnswerText { get; set; }
    }
}
