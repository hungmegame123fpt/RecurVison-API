using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTO.AiClient
{
    public class SubmitAnswerResponse
    {
        public int InterviewId { get; set; }
        public int QuestionId { get; set; }
        public decimal? Score { get; set; }
        public string? Feedback { get; set; }
        public string? Summary { get; set; }
        public AiQuestion? NextQuestion { get; set; }
    }
}
