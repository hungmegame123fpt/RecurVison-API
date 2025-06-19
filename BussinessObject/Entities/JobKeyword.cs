using System;
using System.Collections.Generic;

namespace BusinessObject.Entities;

public partial class JobKeyword
{
    public int JobKeywordId { get; set; }

    public int JobId { get; set; }

    public int KeywordId { get; set; }

    public int? Frequency { get; set; }

    public decimal? RelevanceScore { get; set; }

    public virtual JobPosting Job { get; set; } = null!;

    public virtual Keyword Keyword { get; set; } = null!;
}
