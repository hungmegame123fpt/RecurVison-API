using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTO
{
    public class UserProfileResponse
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string Company { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
        public string MemberSince { get; set; } = string.Empty;
        public string PlanStatus { get; set; } = string.Empty;
        public string AccountStatus { get; set; } = string.Empty;
        public int CvsAnalyzed { get; set; }
        public int CvsThisMonth { get; set; }
    }
}
