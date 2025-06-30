using BusinessObject.DTO.InterviewQuestion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTO.VirtualInterview
{
    public class VirtualInterviewDto
    {
        public int InterviewId { get; set; }
        public int UserId { get; set; }
        public int? JobId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string? Status { get; set; }
        public decimal? OverallScore { get; set; }
        public string? RecordingPath { get; set; }
        public string? UserName { get; set; }
        public string? JobTitle { get; set; }
        public List<InterviewQuestionDto> Questions { get; set; } = new List<InterviewQuestionDto>();
    }
}
