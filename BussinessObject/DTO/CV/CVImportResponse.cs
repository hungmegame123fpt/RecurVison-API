using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTO.CV
{
    public class CVImportResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public int? CvId { get; set; }
        public int? VersionId { get; set; }
        public List<string> Warnings { get; set; } = new List<string>();
        public CVMetadata? Metadata { get; set; }
    }
}
