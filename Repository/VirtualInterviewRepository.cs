using BusinessObject;
using BusinessObject.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class VirtualInterviewRepository : BaseRepository<VirtualInterview>
    {
        public VirtualInterviewRepository(RecurVisionV1Context db) : base(db)
        {
        }

    }
}
