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
using BusinessObject.Entities;

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
        public IUserRoleRepository UserRoleRepository { get; set; }
        public ICvAnalysisResultRepository CvAnalysisResult { get; set; }
        public ICareerPlanRepository CareerPlanRepository { get; set; }
        public IJobPostingRepository JobPostingRepository { get; set; }
        public IBlogPostRepository BlogPostRepository { get; set; }
        public IBlogCategoryRepository BlogCategoryRepository { get; set; }
        public IBaseRepository<CvSkill> CvSkill { get; set; }
        public IBaseRepository<CvProject> CvProject { get; set; }
        public IBaseRepository<CvProjectTechStack> CvProjectTechStack { get; set; }
        public IBaseRepository<CvCertification> CvCertification { get; set; }
        public IBaseRepository<CvEducation> CvEducation { get; set; }
        public IBaseRepository<JobDescription> JobDescriptionRepository { get; set; }
        public IBaseRepository<Author> AuthorRepository { get; set; }
        public UnitOfWork(RecurVisionV1Context db, IConfiguration configuration)
        {
            _db = db;
            _configuration = configuration;
            //_mapper = mapper;
            CVRepository = new CVRepository(_db);
            SubscriptionPlanRepository = new SubscriptionPlanRepository(_db);
            UserRepository = new UserRepository(_db);
            UserRoleRepository = new UserRoleRepository(_db);
            UserSubscriptionRepository = new UserSubscriptionRepository(_db);
            JobFieldRepository = new JobFieldRepository(_db);
            CvAnalysisRepository = new CvAnalysisRepository(_db);
            CvVersionRepository = new CvVersionRepository(_db);
            VirtualInterviewRepository = new VirtualInterviewRepository(_db);
            InterviewQuestionRepository = new InterviewQuestionRepository(_db);
			CvAnalysisResult = new CvAnalysisResultRepository(_db);
			CareerPlanRepository = new CareerPlanRepository(_db);
			JobPostingRepository = new JobPostingRepository(_db);
			BlogPostRepository = new BlogPostRepository(_db);
			BlogCategoryRepository = new BlogCategoryRepository(_db);
            JobDescriptionRepository = new BaseRepository<JobDescription>(_db);
            CvSkill = new BaseRepository<CvSkill>(_db);
            CvCertification = new BaseRepository<CvCertification>(_db);
            CvProject = new BaseRepository<CvProject>(_db);
            CvEducation = new BaseRepository<CvEducation>(_db);
            CvProjectTechStack = new BaseRepository<CvProjectTechStack>(_db);
            AuthorRepository = new BaseRepository<Author>(_db);
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
