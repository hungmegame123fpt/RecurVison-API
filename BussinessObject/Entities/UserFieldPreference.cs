using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Entities
{
    public partial class UserFieldPreference
    {
        public int PreferenceId { get; set; }
        public int UserId { get; set; }
        public int FieldId { get; set; }

        public string? ExperienceLevel { get; set; } // Add validation if needed
        public int? PriorityRank { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        public User User { get; set; } = null!;
        public JobField JobField { get; set; } = null!;
    }
}
