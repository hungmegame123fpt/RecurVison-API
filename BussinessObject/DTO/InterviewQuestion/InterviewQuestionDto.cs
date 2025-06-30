using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTO.InterviewQuestion
{
    public class InterviewQuestionDto
    {
        public int QuestionId { get; set; }
        public int InterviewId { get; set; }
        public string? QuestionText { get; set; }
        public string? AnswerText { get; set; }
        public string? Feedback { get; set; }
        public decimal? QuestionScore { get; set; }
    }
}
