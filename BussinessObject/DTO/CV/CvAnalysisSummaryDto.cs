using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTO.CV
{
    public class CvAnalysisSummaryDto
    {
        public int Id { get; set; }
        public int? Score { get; set; }
        public DateTime CreatedAt { get; set; }

        public CvSimpleDto Cv { get; set; } = null!;
        public JobDescriptionSimpleDto Jd { get; set; } = null!;
    }
}
