using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTO
{
    public class UpdateFeedbackRequest
    {
        public int FeedbackId { get; set; }
        public string Content { get; set; } = string.Empty;
        public int? Rating { get; set; }
    }
}
