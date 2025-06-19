using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTO.Email
{
    public class MailRequest
    {
        public string Email { get; set; }
        public string Subject { get; set; }
        public string Emailbody { get; set; }
    }
}
