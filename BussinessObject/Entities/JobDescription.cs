using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Entities
{
    public class JobDescription
    {
        public int Id { get; set; }
        public string FileName { get; set; } = null!;
        public string FileUrl { get; set; } = null!;
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

        public virtual ICollection<CvAnalysisResult> AnalysisResults { get; set; } = new List<CvAnalysisResult>();
    }
}
