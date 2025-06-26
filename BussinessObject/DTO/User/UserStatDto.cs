using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTO.User
{
    public class UserStatsDto
    {
        public int TotalUsers { get; set; }
        public int ActiveUsers { get; set; }
        public int SuspendedUsers { get; set; }
        public int NewSignupsToday { get; set; }
        public int NewSignupsThisWeek { get; set; }
        public int NewSignupsThisMonth { get; set; }
    }
}
