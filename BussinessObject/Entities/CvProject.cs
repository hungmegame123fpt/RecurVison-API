using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Entities
{
    public partial class CvProject
    {
        public int Id { get; set; }
        public int CvAnalysisResultId { get; set; }

        public string? Title { get; set; }
        public string? Description { get; set; }

        public virtual CvAnalysisResult CvAnalysisResult { get; set; } = null!;
        public virtual ICollection<CvProjectTechStack> TechStacks { get; set; } = new List<CvProjectTechStack>();
    }
}
