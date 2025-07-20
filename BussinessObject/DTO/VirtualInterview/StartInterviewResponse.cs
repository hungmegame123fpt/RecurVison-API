using BusinessObject.DTO.InterviewQuestion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTO.VirtualInterview
{
    public class StartInterviewResponse
    {
        public int InterviewId { get; set; }
        public string SessionId { get; set; }
        public List<InterviewQuestionDto> Questions { get; set; } = new();
    }
}
