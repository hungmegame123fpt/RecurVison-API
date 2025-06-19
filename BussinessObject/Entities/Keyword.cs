using System;
using System.Collections.Generic;

namespace BusinessObject.Entities;

public partial class Keyword
{
    public int KeywordId { get; set; }

    public string Keyword1 { get; set; } = null!;

    public decimal? ImportanceScore { get; set; }

    public string? Category { get; set; }

    public virtual ICollection<CvKeywordMatch> CvKeywordMatches { get; set; } = new List<CvKeywordMatch>();

    public virtual ICollection<JobKeyword> JobKeywords { get; set; } = new List<JobKeyword>();
}
