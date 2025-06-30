using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTO.InterviewQuestion
{
    public class UpdateQuestionScoreDto
    {
        public decimal QuestionScore { get; set; }
        public string? Feedback { get; set; }
    }
}
