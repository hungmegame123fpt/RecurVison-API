using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTO.VirtualInterview
{
    public class StartInterviewRequest
    {
        public int UserId { get; set; }
        public int CvId { get; set; }
        public string JobDescription { get; set; } = string.Empty;
    }
}
