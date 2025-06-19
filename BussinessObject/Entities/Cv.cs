using System;
using System.Collections.Generic;

namespace BusinessObject.Entities;

public partial class Cv
{
    public int CvId { get; set; }

    public int UserId { get; set; }

    public string? Title { get; set; }

    public string? FilePath { get; set; }

    public DateTime? UploadedAt { get; set; }

    public string? Status { get; set; }

    public int? CurrentVersion { get; set; }

    public DateTime? LastModified { get; set; }

    public virtual ICollection<CvKeywordMatch> CvKeywordMatches { get; set; } = new List<CvKeywordMatch>();

    public virtual ICollection<CvVersion> CvVersions { get; set; } = new List<CvVersion>();

    public virtual User User { get; set; } = null!;
}
