using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTO.SubscriptionPlan
{
    public class PlanPopularityDto
    {
        public int PlanId { get; set; }
        public string PlanName { get; set; } = string.Empty;
        public int SubscriptionCount { get; set; }
        public decimal? Revenue { get; set; }
    }
}
