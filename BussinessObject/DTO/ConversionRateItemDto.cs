using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTO
{
    public class ConversionRateItemDto
    {
        public string PlanName { get; set; } = string.Empty;
        public int UserCount { get; set; }
        public double Percentage { get; set; }
    }
}
