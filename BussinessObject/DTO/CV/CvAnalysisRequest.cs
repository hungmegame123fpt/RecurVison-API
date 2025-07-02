using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTO.CV
{
    public class CvAnalysisRequest
    {
        [Required]
        public int CvVersionId { get; set; }

        [Required]
        public IFormFile File { get; set; }

    }
}
