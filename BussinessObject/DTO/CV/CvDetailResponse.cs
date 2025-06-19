using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTO.CV
{
    public class CvDetailResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public CvDetailDto Cv { get; set; }
    }
}
