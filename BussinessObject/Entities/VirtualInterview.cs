using System;
using System.Collections.Generic;

namespace BusinessObject.Entities;

public partial class VirtualInterview
{
    public int InterviewId { get; set; }

    public int UserId { get; set; }

    public int? JobId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public string? Status { get; set; }

    public decimal? OverallScore { get; set; }

    public string? RecordingPath { get; set; }

    public virtual ICollection<InterviewQuestion> InterviewQuestions { get; set; } = null!;

    public virtual JobPosting? Job { get; set; }

    public virtual User User { get; set; } = null!;
}
