using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTO
{
    public class UpdateCareerPlanRequest
    {
        public string? CurrentPosition { get; set; }

        public DateTime? LastUpdated { get; set; }

        public List<MilestoneUpdateDto> Milestones { get; set; } = new();
    }

}
