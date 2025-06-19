using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTO.SubscriptionPlan
{
    public class SubscriptionPlanDto
    {
        public int PlanId { get; set; }
        public string PlanName { get; set; } = null!;
        public string? UserType { get; set; }
        public decimal? Price { get; set; }
        public string? BillingCycle { get; set; }
        public string? Features { get; set; }
        public bool? IsActive { get; set; }
        public int ActiveSubscriptionCount { get; set; }
    }
}
