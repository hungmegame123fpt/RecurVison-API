using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTO.UserSubscription
{
    public class CreateUserSubscriptionDto
    {
        public int UserId { get; set; }
        public int PlanId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsAutoRenew { get; set; } = true;
        public string PaymentStatus { get; set; } = "Active";
    }
}
