using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace BusinessObject.Entities;

public partial class UserSubscription
{
    public int SubscriptionId { get; set; }

    public int? UserId { get; set; }

    public int PlanId { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public bool? IsAutoRenew { get; set; }

    public string? PaymentStatus { get; set; }

    public DateTime? LastPaymentDate { get; set; }
    public int? InterviewPerDayRemaining { get; set; } = 5;
    public int? VoiceInterviewRemaining{ get; set; } = 5;
    public int? CvRemaining { get; set; } = 5;
    public DateTime? LastQuotaResetDate { get; set; } = DateTime.Now;
    [JsonIgnore]

    public virtual SubscriptionPlan Plan { get; set; } = null!;

    public virtual User? User { get; set; }
}
