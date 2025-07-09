using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Entities
{
    public partial class CvSkill
    {
        public int Id { get; set; }
        public int CvAnalysisResultId { get; set; }
        public string SkillName { get; set; } = null!;
		public virtual CvAnalysisResult CvAnalysisResult { get; set; } = null!;
	}
}
