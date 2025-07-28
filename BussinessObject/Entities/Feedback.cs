using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Entities
{
    public class Feedback
    {
        public int FeedbackId { get; set; }

        public int UserId { get; set; } // who gave the feedback
        public string Content { get; set; } = string.Empty;
        public int? Rating { get; set; } // Optional: 1–5 stars
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public virtual User User { get; set; } = null!;
    }
}
