using AutoMapper;
using BusinessObject;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly RecurVisionV1Context _db;
        private readonly IConfiguration _configuration;
        //private readonly IMapper _mapper;
        public ICVRepository CVRepository { get; set; }
        public ISubscriptionPlanRepository SubscriptionPlanRepository { get; set; }
        public IUserRepository UserRepository { get; set; }
        public IUserSubscriptionRepository UserSubscriptionRepository { get; set; }
        public IVirtualInterviewRepository VirtualInterviewRepository { get; set; }
        public IInterviewQuestionRepository InterviewQuestionRepository { get; set; }
        public IKeywordRepository KeywordRepository { get; set; }
        public IJobFieldRepository JobFieldRepository { get; set; }
        public ICvAnalysisRepository CvAnalysisRepository { get; set; }
        public ICvVersionRepository CvVersionRepository { get; set; }
        public UnitOfWork(RecurVisionV1Context db, IConfiguration configuration)
        {
            _db = db;
            _configuration = configuration;
            //_mapper = mapper;
            CVRepository = new CVRepository(_db);
            SubscriptionPlanRepository = new SubscriptionPlanRepository(_db);
            UserRepository = new UserRepository(_db);
            UserSubscriptionRepository = new UserSubscriptionRepository(_db);
            JobFieldRepository = new JobFieldRepository(_db);
            CvAnalysisRepository = new CvAnalysisRepository(_db);
            CvVersionRepository = new CvVersionRepository(_db);
        }
        public async Task SaveChanges()
        {
            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                throw new Exception(ex.Message);
            }

        }
        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await _db.Database.BeginTransactionAsync();
        }

        public async Task CommitAsync()
        {
            await _db.Database.CurrentTransaction?.CommitAsync();
        }

        public async Task RollbackAsync()
        {
            await _db.Database.CurrentTransaction?.RollbackAsync();
        }
    }
}
