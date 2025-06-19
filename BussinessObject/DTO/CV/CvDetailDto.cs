using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTO.CV
{
    public class CvDetailDto
    {
        public int CvId { get; set; }
        public int UserId { get; set; }
        public string Title { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? CurrentVersion { get; set; }
        public string FilePath { get; set; }
        public List<CvVersionDto> Versions { get; set; } = new List<CvVersionDto>();
    }
}
