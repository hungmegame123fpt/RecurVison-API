using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTO.VirtualInterview
{
    public class CreateVirtualInterviewDto
    {
        public int UserId { get; set; }
        public int? JobId { get; set; }
    }
}
