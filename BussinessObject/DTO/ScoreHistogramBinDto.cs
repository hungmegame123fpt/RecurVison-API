using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTO
{
    public class ScoreHistogramBinDto
    {
        public string Range { get; set; } = string.Empty;
        public int Count { get; set; }
    }
}
