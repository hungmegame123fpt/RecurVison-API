using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTO.Payment
{
    public class CreatePaymentLinkRequest
    {
        public int UserId { get; set; }
        public int PlanId { get; set; }
        public bool IsAutoRenew { get; set; } = false;
        public string? Description { get; set; }
        public string ReturnUrl { get; set; } = null!;
        public string CancelUrl { get; set; } = null!;
    }
}
