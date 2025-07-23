using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTO
{
    public class UpdateProfileRequest
    {
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string PhoneNumber { get; set; } = "";
        public string Location { get; set; } = "";
        public string Company { get; set; } = "";
        public string Position { get; set; } = "";
    }
}
