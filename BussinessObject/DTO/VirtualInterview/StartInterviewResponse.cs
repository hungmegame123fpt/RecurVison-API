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
        public List<InterviewQuestionDto> Questions { get; set; } = new();
        public string? Analysis {  get; set; } = string.Empty;
        public List<string> NextFocusAreas { get; set; } = new List<string>();
        public string CleanCvText { get; set; } = string.Empty;
        public string JobDescription { get; set; } = string.Empty;
    }
}
