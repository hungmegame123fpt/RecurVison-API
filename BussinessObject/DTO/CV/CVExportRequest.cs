using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTO.CV
{
    public class CVExportRequest
    {
        [Required]
        public int CvId { get; set; }

        [Required]
        public int UserId { get; set; }

        public string ExportFormat { get; set; } = "pdf"; // pdf, docx, txt

        public bool IncludeAnalysis { get; set; } = false;
    }
}
