using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Entities
{
    public partial class CvAnalysisResult
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Summary { get; set; }

        public int CvId { get; set; } 
        public string? JobDescriptionFileName { get; set; }
        public DateTime CreatedAt { get; set; }

        public int? MatchScore { get; set; }
        public string? JdAlignment { get; set; }
        public string? ImprovementSuggestions { get; set; }
        public virtual Cv Cv { get; set; } = null!;
        public virtual ICollection<CvSkill> Skills { get; set; } = new List<CvSkill>();
        public virtual ICollection<CvEducation> Education { get; set; } = new List<CvEducation>();
        public virtual ICollection<CvProject> Projects { get; set; } = new List<CvProject>();
        public virtual ICollection<CvCertification> Certifications { get; set; } = new List<CvCertification>();
    }
}
