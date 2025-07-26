using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTO
{
    public class NextCareerGoalDto
    {
        public int? PlanId { get; set; }
        public int? MilestoneId { get; set; }
        public string TargetTitle { get; set; } = "Land a new job with desired offer";
        public string? TargetDate { get; set; } 
        public string? OverallPath { get; set; } 
        public string? CurrentPosition { get; set; } 
        public string? Status { get; set; } 
    }
}
