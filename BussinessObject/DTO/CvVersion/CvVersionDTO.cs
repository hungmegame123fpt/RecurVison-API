using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTO.CvVersion
{
    public class CvVersionDTO
    {
        public int VersionId { get; set; }

        public int CvId { get; set; }

        public int? VersionNumber { get; set; }

        public string? FilePath { get; set; }

        public DateTime? CreatedAt { get; set; }

        public string? ChangeSummary { get; set; }

        public decimal? AiScore { get; set; }
        public string? PlainText { get; set; }
    }
}
