using System;
using System.Collections.Generic;

namespace BusinessObject.Entities;

public partial class CareerMilestone
{
    public int MilestoneId { get; set; }

    public int PlanId { get; set; }

    public string? Title { get; set; }

    public string? Description { get; set; }

    public int? TargetYear { get; set; }

    public string? RequiredSkills { get; set; }

    public string? AchievementStatus { get; set; }

    public virtual CareerPlan Plan { get; set; } = null!;
}
