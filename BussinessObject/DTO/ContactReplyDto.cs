using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTO
{
    public class ContactReplyDto
    {
        public int ContactId { get; set; }
        public string Response { get; set; } = string.Empty;
    }
}
