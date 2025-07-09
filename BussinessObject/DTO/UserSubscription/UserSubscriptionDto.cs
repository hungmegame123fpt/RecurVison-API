using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTO.UserSubscription
{
    public class UserSubscriptionDto
    {
        public int SubscriptionId { get; set; }
        public int? UserId { get; set; }
        public string? UserName { get; set; }
        public string? UserEmail { get; set; }
        public int PlanId { get; set; }
        public string? PlanName { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool? IsAutoRenew { get; set; }
        public string? PaymentStatus { get; set; }
        public DateTime? LastPaymentDate { get; set; }
        public decimal? PlanPrice { get; set; }
        public string? BillingCycle { get; set; }
        public int? InterviewPerDayRemaining { get; set; } = 5;
        public int? VoiceInterviewRemaining { get; set; } = 5;
        public int? CvRemaining { get; set; } = 5;
        public DateTime? LastQuotaResetDate { get; set; } = DateTime.Now;
    }
}
