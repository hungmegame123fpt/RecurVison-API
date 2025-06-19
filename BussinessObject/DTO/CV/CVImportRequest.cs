using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTO.CV
{
    public class CVImportRequest
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        public IFormFile File { get; set; }

        public string? Title { get; set; }

        public bool CreateNewVersion { get; set; } = false;

        public int? ExistingCvId { get; set; }
    }
}
