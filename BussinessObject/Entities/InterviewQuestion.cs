using System;
using System.Collections.Generic;

namespace BusinessObject.Entities;

public partial class InterviewQuestion
{
    public int QuestionId { get; set; }

    public int InterviewId { get; set; }

    public string? QuestionText { get; set; }

    public string? AnswerText { get; set; }

    public string? Feedback { get; set; }

    public decimal? QuestionScore { get; set; }

    public virtual VirtualInterview Interview { get; set; } = null!;
}
