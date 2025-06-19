using BusinessObject.DTO.CV;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interface
{
    public interface IDocumentParserService 
    {
        Task<ParsedDocumentResult> ParseDocumentAsync(string filePath, string fileExtension = null);
    }
}
