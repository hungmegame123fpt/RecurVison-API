using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTO.SubscriptionPlan
{
    public class PlanFilterDto
    {
        public string? UserType { get; set; }
        public string? BillingCycle { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public bool? IsActive { get; set; }
        public string? SearchTerm { get; set; }
        public string? SortBy { get; set; } = "Price"; 
        public string? SortOrder { get; set; } = "ASC";
    }
}
