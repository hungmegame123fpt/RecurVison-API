using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTO
{
    public class PremiumConversionRateDto
    {
        public int TotalUsers { get; set; }
        public List<ConversionRateItemDto> Breakdown { get; set; } = new();
    }
}
