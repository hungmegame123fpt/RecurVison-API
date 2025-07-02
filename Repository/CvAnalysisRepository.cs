using BusinessObject;
using BusinessObject.Entities;
using Microsoft.EntityFrameworkCore;
using Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class CvAnalysisRepository : BaseRepository<CvAnalysisFile>,ICvAnalysisRepository
    {
        public CvAnalysisRepository(RecurVisionV1Context db) : base(db)
        {
        }
    }
}
