using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTO
{
    public class StatsComparisonDto
    {
        public int Today { get; set; }
        public int Yesterday { get; set; }
        public double PercentageChange { get; set; } // +/- %
    }
}
