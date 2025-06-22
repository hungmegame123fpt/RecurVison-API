using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTO.User
{
    public class UserDto
    {
        public int UserId { get; set; }

        public string Email { get; set; } = null!;

        public string Password { get; set; } = null;

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? LastLogin { get; set; }

        public string? AccountStatus { get; set; }

        public string? GoogleId { get; set; }
        public bool? EmailVerified { get; set; } = false;

        public string? RegistrationSource { get; set; }

        public string? ProfilePhotoPath { get; set; }

        public string? SubscriptionStatus { get; set; }
    }
}
