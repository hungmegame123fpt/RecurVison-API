using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Entities
{
    public class CvAnalysisFile
    {
        public int Id { get; set; }

        public int CvVersionId { get; set; }                

        public string FileUrl { get; set; } = null!;         
        public string PublicId { get; set; } = null!;        
        public string FileType { get; set; } = "json";      
        public string Category { get; set; } = "cv_analysis";

        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

        public virtual CvVersion CvVersion { get; set; } = null!;
        
    }
}
