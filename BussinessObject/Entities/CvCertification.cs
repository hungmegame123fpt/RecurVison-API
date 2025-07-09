using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Entities
{
    public partial class CvCertification
    {
        public int Id { get; set; }
        public int CvAnalysisResultId { get; set; }

        public string? Name { get; set; }
        public string? Issuer { get; set; }
        public string? TimePeriod { get; set; }
        public string? Description { get; set; }

        public virtual CvAnalysisResult CvAnalysisResult { get; set; } = null!;
    }
}
