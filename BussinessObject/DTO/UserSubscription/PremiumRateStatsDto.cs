using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTO.UserSubscription
{
    public class PremiumRateStatsDto
    {
        public PremiumRateDetail AllTime { get; set; } = new();
        public PremiumRateDetail Today { get; set; } = new();
        public PremiumRateDetail ThisWeek { get; set; } = new();
        public PremiumRateDetail ThisMonth { get; set; } = new();
    }
}
