using System;
using System.Collections.Generic;

namespace BusinessObject.Entities;

public partial class JobPosting
{
    public int JobId { get; set; }

    public int UserId { get; set; }

    public string? JobPosition { get; set; }

    public string? CompanyName { get; set; }

    public string? Location { get; set; }

    public string? JobDescription { get; set; }

    public decimal? MaxSalary { get; set; }

    public string? JobType { get; set; }

    public string? Status { get; set; }

    public DateTime? DateSaved { get; set; }

    public DateTime? DateApplied { get; set; }

    public DateTime? Deadline { get; set; }

    public int? ExcitementLevel { get; set; }

    public string? Notes { get; set; }

    public bool? IsSelected { get; set; }

    public virtual ICollection<CvKeywordMatch> CvKeywordMatches { get; set; } = new List<CvKeywordMatch>();

    public virtual ICollection<JobKeyword> JobKeywords { get; set; } = new List<JobKeyword>();

    public virtual User User { get; set; } = null!;

    public virtual ICollection<VirtualInterview> VirtualInterviews { get; set; } = new List<VirtualInterview>();
}
