using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTO.SubscriptionPlan
{
    public class PlanStatsDto
    {
        public int? TotalPlans { get; set; }
        public int? ActivePlans { get; set; }
        public int? InactivePlans { get; set; }
        public int? TotalActiveSubscriptions { get; set; }
        public decimal? TotalMonthlyRevenue { get; set; }
        public decimal? TotalYearlyRevenue { get; set; }
        public List<PlanPopularityDto> MostPopularPlans { get; set; } = new();
    }
}
