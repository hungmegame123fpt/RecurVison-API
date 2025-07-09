using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Entities
{
    public partial class CvProjectTechStack
    {
        public int Id { get; set; }
        public int CvProjectId { get; set; }
        public string TechName { get; set; } = null!;
        public virtual CvProject CvProject { get; set; } = null!;
    }
}
