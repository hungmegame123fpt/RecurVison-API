using System;
using System.Collections.Generic;

namespace BusinessObject.Entities;

public partial class CareerPlan
{
    public int PlanId { get; set; }

    public int UserId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? LastUpdated { get; set; }

    public string? CareerGoal { get; set; }

    public string? CurrentPosition { get; set; }

    public int? TimelineYears { get; set; }

    public virtual ICollection<CareerMilestone> CareerMilestones { get; set; } = new List<CareerMilestone>();

    public virtual User User { get; set; } = null!;
}
