using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTO.UserSubscription
{
    public class PremiumRateDetail
    {
        public int TotalUsers { get; set; }
        public int PremiumUsers { get; set; }
        public decimal Rate { get; set; } // %
    }
}
