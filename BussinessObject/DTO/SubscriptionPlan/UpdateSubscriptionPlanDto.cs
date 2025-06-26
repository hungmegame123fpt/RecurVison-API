using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTO.SubscriptionPlan
{
    public class UpdateSubscriptionPlanDto
    {
        [Required]
        [StringLength(100, MinimumLength = 3)]
        public string PlanName { get; set; } = null!;

        [StringLength(50)]
        public string? UserType { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Price must be a positive value")]
        public decimal? Price { get; set; }

        [RegularExpression("^(monthly|quarterly|yearly|weekly)$", ErrorMessage = "Billing cycle must be: monthly, quarterly, yearly, or weekly")]
        public string? BillingCycle { get; set; }

        [StringLength(1000)]
        public string? Features { get; set; }

        public bool IsActive { get; set; }
    }
}

