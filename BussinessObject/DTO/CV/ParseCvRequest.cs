using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTO.CV
{
    public class ParseCvRequest
    {
        [Required]
        public IFormFile File { get; set; }

        public bool IncludeMetadata { get; set; } = true;
        public bool ExtractSections { get; set; } = true;
        public string? ParseOptions { get; set; } // JSON string for additional options
    }
}
