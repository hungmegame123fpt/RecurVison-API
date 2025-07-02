using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTO.CV
{
    public class CvAnalysisResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = "";
        public string? FileUrl { get; set; }
    }
}
