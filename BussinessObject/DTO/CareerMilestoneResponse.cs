using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTO
{
    public class CareerMilestoneResponse
    {
        public string Title { get; set; } = string.Empty;
        public int? YearToComplete { get; set; }
    }
}
