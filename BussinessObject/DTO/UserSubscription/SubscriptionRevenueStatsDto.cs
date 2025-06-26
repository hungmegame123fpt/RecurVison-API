using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTO.UserSubscription
{
    public class SubscriptionRevenueStatsDto
    {
        public decimal TotalRevenue { get; set; }
        public decimal MonthlyRevenue { get; set; }
        public decimal YearlyRevenue { get; set; }
        public int ActiveSubscriptions { get; set; }
        public int CancelledSubscriptions { get; set; }
        public int ExpiredSubscriptions { get; set; }
        public List<MonthlyRevenueDto> MonthlyBreakdown { get; set; } = new();
        public List<PlanRevenueDto> RevenueByPlan { get; set; } = new();
    }
}
