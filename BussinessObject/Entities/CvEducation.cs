using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Entities
{
    public partial class CvEducation
    {
        public int Id { get; set; }
        public int CvAnalysisResultId { get; set; }

        public string? Degree { get; set; }
        public string? Institution { get; set; }
        public int? StartYear { get; set; }
        public int? EndYear { get; set; }
        public string? Description { get; set; }

        public virtual CvAnalysisResult CvAnalysisResult { get; set; } = null!;
    }
}
