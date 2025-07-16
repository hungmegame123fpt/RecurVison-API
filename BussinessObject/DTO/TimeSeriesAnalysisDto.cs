using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTO
{
    public class TimeSeriesAnalysisDto
    {
        public List<TimeSeriesPointDto> Data { get; set; } = new();
        public TimeSeriesPointDto? Peak { get; set; }
        public TimeSeriesPointDto? Low { get; set; }
    }
}
