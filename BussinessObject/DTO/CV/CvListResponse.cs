using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTO.CV
{
    public class CvListResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public List<CVDto> Cvs { get; set; } = new List<CVDto>();
    }
}
