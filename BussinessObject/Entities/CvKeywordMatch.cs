using System;
using System.Collections.Generic;

namespace BusinessObject.Entities;

public partial class CvKeywordMatch
{
    public int MatchId { get; set; }

    public int CvId { get; set; }

    public int JobId { get; set; }

    public int KeywordId { get; set; }

    public bool? IsPresent { get; set; }

    public decimal? MatchScore { get; set; }

    public virtual Cv Cv { get; set; } = null!;

    public virtual JobPosting Job { get; set; } = null!;

    public virtual Keyword Keyword { get; set; } = null!;
}
