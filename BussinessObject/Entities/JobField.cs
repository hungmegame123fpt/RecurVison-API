using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Entities
{
    public partial class JobField
    {
        public int FieldId { get; set; }
        public string FieldName { get; set; } = string.Empty;
        public int? CategoryId { get; set; }
        public string? CommonSkills { get; set; }
        public string? TypicalKeywords { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public bool IsActive { get; set; } = true;

        public FieldCategory Category { get; set; } = null!;
        public ICollection<UserFieldPreference> UserFieldPreferences { get; set; } = new List<UserFieldPreference>();
    }
}
