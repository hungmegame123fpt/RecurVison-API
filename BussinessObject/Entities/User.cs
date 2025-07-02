using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace BusinessObject.Entities;

public partial class User
{
    public int UserId { get; set; }

    public string Email { get; set; } = null!;

    public string? Password { get; set; } 

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? LastLogin { get; set; }

    public string? AccountStatus { get; set; }

    public string? GoogleId { get; set; }
    public bool? EmailVerified { get; set; } = false;

    public string? RegistrationSource { get; set; }

    public string? ProfilePhotoPath { get; set; }

    public string? SubscriptionStatus { get; set; }

    public virtual ICollection<CareerPlan> CareerPlans { get; set; } = new List<CareerPlan>();

    public virtual ICollection<Cv> Cvs { get; set; } = new List<Cv>();

    public virtual ICollection<JobPosting> JobPostings { get; set; } = new List<JobPosting>();

    public virtual ICollection<UserCharacteristic> UserCharacteristics { get; set; } = new List<UserCharacteristic>();

    public virtual ICollection<UserRole> UserRoleAssignedByNavigations { get; set; } = new List<UserRole>();

    public virtual ICollection<UserRole> UserRoleUsers { get; set; } = new List<UserRole>();
    [JsonIgnore]
    public virtual ICollection<UserSubscription> UserSubscriptions { get; set; } = new List<UserSubscription>();

    public virtual ICollection<VirtualInterview> VirtualInterviews { get; set; } = new List<VirtualInterview>();
    public virtual ICollection<UserFieldPreference> UserFieldPreferences { get; set; } = new List<UserFieldPreference>();
}
