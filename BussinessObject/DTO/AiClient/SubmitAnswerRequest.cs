using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTO.AiClient
{
    public class SubmitAnswerRequest
    {
        public int InterviewId { get; set; }
        public int QuestionId { get; set; }
        public string AnswerText { get; set; } = string.Empty;
        public string CleanCvText { get; set; } = string.Empty;
        public string JobDescription { get; set; } = string.Empty;
    }
}
