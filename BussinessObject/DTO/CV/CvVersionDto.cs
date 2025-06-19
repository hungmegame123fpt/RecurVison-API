using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTO.CV
{
    public class CvVersionDto
    {
        public int VersionId { get; set; }
        public int? VersionNumber { get; set; }
        public string FilePath { get; set; }
        public string ChangeSummary { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
