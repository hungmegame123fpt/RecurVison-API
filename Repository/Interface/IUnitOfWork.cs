using BusinessObject.Entities;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interface
{
    public interface IUnitOfWork
    {
        ICVRepository CVRepository { get; }
        IUserRepository UserRepository { get; }
        IUserRoleRepository UserRoleRepository { get; }
        IUserSubscriptionRepository UserSubscriptionRepository { get; }
        ISubscriptionPlanRepository SubscriptionPlanRepository { get; }
        IVirtualInterviewRepository VirtualInterviewRepository { get;  }
        IInterviewQuestionRepository InterviewQuestionRepository { get;  }
        IKeywordRepository KeywordRepository { get;  }
        IJobFieldRepository JobFieldRepository { get;  }
        ICvAnalysisRepository CvAnalysisRepository { get;  }
        ICvVersionRepository CvVersionRepository { get;  }
		ICvAnalysisResultRepository CvAnalysisResult { get;  }
		ICareerPlanRepository CareerPlanRepository { get;  }
		IJobPostingRepository JobPostingRepository { get;  }
		IBlogCategoryRepository BlogCategoryRepository { get;  }
		IBlogPostRepository BlogPostRepository { get;  }
        IBaseRepository<JobDescription> JobDescriptionRepository { get; }
        IBaseRepository<CvSkill> CvSkill { get; }
        IBaseRepository<CvCertification> CvCertification { get; }
        IBaseRepository<CvProject> CvProject { get; }
        IBaseRepository<CvEducation> CvEducation { get; }
        IBaseRepository<CvProjectTechStack> CvProjectTechStack { get; }
        IBaseRepository<Author> AuthorRepository { get; }

        Task SaveChanges();
        Task<IDbContextTransaction> BeginTransactionAsync();
        Task CommitAsync();
        Task RollbackAsync();
    }
}
