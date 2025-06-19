using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTO.Payment
{
    public class PaymentWebhookRequest
    {
        public long OrderCode { get; set; }
        public string Status { get; set; } = null!;
        public decimal Amount { get; set; }
        public string? Description { get; set; }
    }
}
