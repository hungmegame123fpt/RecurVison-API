using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTO.VirtualInterview
{
    public class UpdateInterviewStatusDto
    {
        public string Status { get; set; } = string.Empty;
        public decimal? OverallScore { get; set; }
        public string? RecordingPath { get; set; }
    }
}
