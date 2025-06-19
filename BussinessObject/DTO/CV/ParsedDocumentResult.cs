using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTO.CV
{
    public class ParsedDocumentResult
    {
        public List<string> ExtractedSections { get; set; } = new();
        public List<string> Warnings { get; set; } = new();
        public string? PlainTextContent { get; set; }
    }
}
