using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTO.CV
{
    public class CVMetadata
    {
        public string? OriginalFileName { get; set; }
        public long FileSize { get; set; }
        public string? FileType { get; set; }
        public int PageCount { get; set; }
        public DateTime ProcessedAt { get; set; }
        public List<string> ExtractedSections { get; set; } = new List<string>();
    }
}
