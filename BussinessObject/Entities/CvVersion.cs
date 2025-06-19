using System;
using System.Collections.Generic;

namespace BusinessObject.Entities;

public partial class CvVersion
{
    public int VersionId { get; set; }

    public int CvId { get; set; }

    public int? VersionNumber { get; set; }

    public string? FilePath { get; set; }

    public DateTime? CreatedAt { get; set; }

    public string? ChangeSummary { get; set; }

    public decimal? AiScore { get; set; }

    public virtual Cv Cv { get; set; } = null!;
}
