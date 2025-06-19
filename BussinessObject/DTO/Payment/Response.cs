using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTO.Payment
{
    public class Response
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public string? PaymentUrl { get; set; }
        public long? OrderCode { get; set; }
        public int? SubscriptionId { get; set; }
    }
}
