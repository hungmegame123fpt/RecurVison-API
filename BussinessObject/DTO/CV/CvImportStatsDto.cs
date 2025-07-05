using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTO.CV
{
    public class CvImportStatsDto
    {
        public int Total { get; set; }
        public int ImportedToday { get; set; }
        public int ImportedThisWeek { get; set; }
        public int ImportedThisMonth { get; set; }
    }
}
