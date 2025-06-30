using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTO.InterviewQuestion
{
    public class CreateInterviewQuestionDto
    {
        public int InterviewId { get; set; }
        public string QuestionText { get; set; } = string.Empty;
    }
}
