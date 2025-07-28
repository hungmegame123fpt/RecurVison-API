using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTO
{
    public class TopCustomerDTO
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string SubscriptionPlan { get; set; } = string.Empty;
        public decimal TotalSpent { get; set; }
        public DateTime LastPaymentDate { get; set; }
    }
}
