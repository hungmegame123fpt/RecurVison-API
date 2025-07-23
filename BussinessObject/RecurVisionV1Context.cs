using System;
using System.Collections.Generic;
using BusinessObject.Entities;
using Microsoft.EntityFrameworkCore;

namespace BusinessObject;

public partial class RecurVisionV1Context : DbContext
{
    public RecurVisionV1Context()
    {
    }

    public RecurVisionV1Context(DbContextOptions<RecurVisionV1Context> options)
        : base(options)
    {
    }

    public virtual DbSet<CareerMilestone> CareerMilestones { get; set; }

    public virtual DbSet<CareerPlan> CareerPlans { get; set; }

    public virtual DbSet<Cv> Cvs { get; set; }

    public virtual DbSet<CvKeywordMatch> CvKeywordMatches { get; set; }

    public virtual DbSet<CvVersion> CvVersions { get; set; }

    public virtual DbSet<InterviewQuestion> InterviewQuestions { get; set; }

    public virtual DbSet<JobKeyword> JobKeywords { get; set; }

    public virtual DbSet<JobPosting> JobPostings { get; set; }

    public virtual DbSet<Keyword> Keywords { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<SubscriptionPlan> SubscriptionPlans { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserCharacteristic> UserCharacteristics { get; set; }

    public virtual DbSet<UserCharacteristicsHistory> UserCharacteristicsHistories { get; set; }

    public virtual DbSet<UserRole> UserRoles { get; set; }

    public virtual DbSet<UserSubscription> UserSubscriptions { get; set; }

    public virtual DbSet<VirtualInterview> VirtualInterviews { get; set; }
    public DbSet<FieldCategory> FieldCategories { get; set; }
    public DbSet<JobField> JobFields { get; set; }
    public DbSet<UserFieldPreference> UserFieldPreferences { get; set; }
    public DbSet<CvAnalysisFile> CvAnalysisFiles { get; set; }
    public DbSet<CvAnalysisResult> CvAnalysisResults { get; set; }
    public DbSet<CvSkill> CvSkills { get; set; }
    public DbSet<CvEducation> CvEducations { get; set; }
    public DbSet<CvProject> CvProjects { get; set; }
    public DbSet<CvProjectTechStack> CvProjectTechStacks { get; set; }
    public DbSet<CvCertification> CvCertifications { get; set; }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Server=tcp:recrudb.database.windows.net,1433;Initial Catalog=RecurVision_V1-2025-7-13-0-38;Persist Security Info=False;User ID=hung;Password=Thinhboro123;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CareerMilestone>(entity =>
        {
            entity.HasKey(e => e.MilestoneId).HasName("PK__CAREER_M__67592EB7F3C93BD0");

            entity.ToTable("CAREER_MILESTONE");

            entity.Property(e => e.MilestoneId)
                .ValueGeneratedOnAdd()
                .HasColumnName("milestone_id");
            entity.Property(e => e.AchievementStatus)
                .HasMaxLength(255)
                .HasColumnName("achievement_status");
            entity.Property(e => e.Description)
                .HasColumnType("text")
                .HasColumnName("description");
            entity.Property(e => e.PlanId).HasColumnName("plan_id");
            entity.Property(e => e.RequiredSkills)
                .HasColumnType("text")
                .HasColumnName("required_skills");
            entity.Property(e => e.TargetYear).HasColumnName("target_year");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .HasColumnName("title");

            entity.HasOne(d => d.Plan).WithMany(p => p.CareerMilestones)
                .HasForeignKey(d => d.PlanId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__CAREER_MI__plan___14270015");
        });

        modelBuilder.Entity<CareerPlan>(entity =>
        {
            entity.HasKey(e => e.PlanId).HasName("PK__CAREER_P__BE9F8F1D0059987E");

            entity.ToTable("CAREER_PLAN");

            entity.Property(e => e.PlanId)
                .ValueGeneratedOnAdd()
                .HasColumnName("plan_id");
            entity.Property(e => e.CareerGoal)
                .HasColumnType("text")
                .HasColumnName("career_goal");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.CurrentPosition)
                .HasMaxLength(255)
                .HasColumnName("current_position");
            entity.Property(e => e.LastUpdated)
                .HasColumnType("datetime")
                .HasColumnName("last_updated");
            entity.Property(e => e.TimelineYears).HasColumnName("timeline_years");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.TargetFieldId).HasColumnName("target_field_id");
            entity.HasOne(d => d.User).WithMany(p => p.CareerPlans)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CAREER_PL__user___1332DBDC");
            entity.HasOne(e => e.TargetField)
                  .WithMany()
                  .HasForeignKey(e => e.TargetFieldId)
                  .HasConstraintName("FK_CAREER_PLAN_FIELD");
        });

        modelBuilder.Entity<Cv>(entity =>
        {
            entity.HasKey(e => e.CvId).HasName("PK__CV__C36883E629F84B2E");

            entity.ToTable("CV");

            entity.Property(e => e.CvId)
                .ValueGeneratedOnAdd()
                .HasColumnName("cv_id");
            entity.Property(e => e.CurrentVersion).HasColumnName("current_version");
            entity.Property(e => e.FilePath)
                .HasMaxLength(255)
                .HasColumnName("file_path");
            entity.Property(e => e.LastModified)
                .HasColumnType("datetime")
                .HasColumnName("last_modified");
            entity.Property(e => e.Status)
                .HasMaxLength(255)
                .HasColumnName("status");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .HasColumnName("title");
            entity.Property(e => e.UploadedAt)
                .HasColumnType("datetime")
                .HasColumnName("uploaded_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.FieldId).HasColumnName("field_id");

            entity.HasOne(d => d.User).WithMany(p => p.Cvs)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CV__user_id__08B54D69");
            entity.HasOne(e => e.TargetField)
                 .WithMany()
                 .HasForeignKey(e => e.FieldId)
                 .HasConstraintName("FK_CV_FIELD");
        });

        modelBuilder.Entity<CvKeywordMatch>(entity =>
        {
            entity.HasKey(e => e.MatchId).HasName("PK__CV_KEYWO__9D7FCBA3B65B269F");

            entity.ToTable("CV_KEYWORD_MATCH");

            entity.Property(e => e.MatchId)
                .ValueGeneratedOnAdd()
                .HasColumnName("match_id");
            entity.Property(e => e.CvId).HasColumnName("cv_id");
            entity.Property(e => e.IsPresent).HasColumnName("is_present");
            entity.Property(e => e.JobId).HasColumnName("job_id");
            entity.Property(e => e.KeywordId).HasColumnName("keyword_id");
            entity.Property(e => e.MatchScore)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("match_score");

            entity.HasOne(d => d.Cv).WithMany(p => p.CvKeywordMatches)
                .HasForeignKey(d => d.CvId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CV_KEYWOR__cv_id__0D7A0286");

            entity.HasOne(d => d.Job).WithMany(p => p.CvKeywordMatches)
                .HasForeignKey(d => d.JobId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CV_KEYWOR__job_i__0E6E26BF");

            entity.HasOne(d => d.Keyword).WithMany(p => p.CvKeywordMatches)
                .HasForeignKey(d => d.KeywordId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CV_KEYWOR__keywo__0F624AF8");
        });

        modelBuilder.Entity<CvVersion>(entity =>
        {
            entity.HasKey(e => e.VersionId).HasName("PK__CV_VERSI__07A5886900B61DCA");

            entity.ToTable("CV_VERSION");

            entity.Property(e => e.VersionId)
                .ValueGeneratedOnAdd()
                .HasColumnName("version_id");
            entity.Property(e => e.AiScore)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("ai_score");
            entity.Property(e => e.ChangeSummary)
                .HasColumnType("text")
                .HasColumnName("change_summary");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.CvId).HasColumnName("cv_id");
            entity.Property(e => e.FilePath)
                .HasMaxLength(255)
                .HasColumnName("file_path");
            entity.Property(e => e.PlainText)
                .HasMaxLength(255)
                .HasColumnName("plain_text");
            entity.Property(e => e.VersionNumber).HasColumnName("version_number");

            entity.HasOne(d => d.Cv).WithMany(p => p.CvVersions)
                .HasForeignKey(d => d.CvId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__CV_VERSIO__cv_id__09A971A2");
        });

        modelBuilder.Entity<InterviewQuestion>(entity =>
        {
            entity.HasKey(e => e.QuestionId).HasName("PK__INTERVIE__2EC215493D220C89");

            entity.ToTable("INTERVIEW_QUESTION");

            entity.Property(e => e.QuestionId)
                .ValueGeneratedOnAdd()
                .HasColumnName("question_id");
            entity.Property(e => e.AnswerText)
                .HasColumnType("nvarchar(500)")
                .HasColumnName("answer_text");
            entity.Property(e => e.Feedback)
                .HasColumnType("nvarchar(500)")
                .HasColumnName("feedback");
            entity.Property(e => e.InterviewId).HasColumnName("interview_id");
            entity.Property(e => e.QuestionScore)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("question_score");
            entity.Property(e => e.QuestionText)
                .HasColumnType("nvarchar(500)")
                .HasColumnName("question_text");

            entity.HasOne(d => d.Interview).WithMany(p => p.InterviewQuestions)
                .HasForeignKey(d => d.InterviewId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__INTERVIEW__inter__123EB7A3");
        });

        modelBuilder.Entity<JobKeyword>(entity =>
        {
            entity.HasKey(e => e.JobKeywordId).HasName("PK__JOB_KEYW__673BADDBC267BC33");

            entity.ToTable("JOB_KEYWORD");

            entity.Property(e => e.JobKeywordId)
                .ValueGeneratedOnAdd()
                .HasColumnName("job_keyword_id");
            entity.Property(e => e.Frequency).HasColumnName("frequency");
            entity.Property(e => e.JobId).HasColumnName("job_id");
            entity.Property(e => e.KeywordId).HasColumnName("keyword_id");
            entity.Property(e => e.RelevanceScore)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("relevance_score");

            entity.HasOne(d => d.Job).WithMany(p => p.JobKeywords)
                .HasForeignKey(d => d.JobId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__JOB_KEYWO__job_i__0B91BA14");

            entity.HasOne(d => d.Keyword).WithMany(p => p.JobKeywords)
                .HasForeignKey(d => d.KeywordId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__JOB_KEYWO__keywo__0C85DE4D");
        });

        modelBuilder.Entity<JobPosting>(entity =>
        {
            entity.HasKey(e => e.JobId).HasName("PK__JOB_POST__6E32B6A54573B47F");

            entity.ToTable("JOB_POSTING");

            entity.Property(e => e.JobId)
                .ValueGeneratedOnAdd()
                .HasColumnName("job_id");
            entity.Property(e => e.CompanyName)
                .HasMaxLength(255)
                .HasColumnName("company_name");
            entity.Property(e => e.DateApplied)
                .HasColumnType("datetime")
                .HasColumnName("date_applied");
            entity.Property(e => e.DateSaved)
                .HasColumnType("datetime")
                .HasColumnName("date_saved");
            entity.Property(e => e.Deadline)
                .HasColumnType("datetime")
                .HasColumnName("deadline");
            entity.Property(e => e.ExcitementLevel).HasColumnName("excitement_level");
            entity.Property(e => e.IsSelected).HasColumnName("is_selected");
            entity.Property(e => e.JobDescription)
                .HasColumnType("text")
                .HasColumnName("job_description");
            entity.Property(e => e.JobPosition)
                .HasMaxLength(255)
                .HasColumnName("job_position");
            entity.Property(e => e.JobType)
                .HasMaxLength(255)
                .HasColumnName("job_type");
            entity.Property(e => e.Location)
                .HasMaxLength(255)
                .HasColumnName("location");
            entity.Property(e => e.MaxSalary)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("max_salary");
            entity.Property(e => e.Notes)
                .HasColumnType("text")
                .HasColumnName("notes");
            entity.Property(e => e.Status)
                .HasMaxLength(255)
                .HasColumnName("status");
            entity.Property(e => e.FieldId)
               .HasMaxLength(255)
               .HasColumnName("field_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.JobPostings)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__JOB_POSTI__user___0A9D95DB");
            entity.HasOne(e => e.JobField)
                 .WithMany()
                 .HasForeignKey(e => e.FieldId)
                 .HasConstraintName("FK_JOB_POSTING_FIELD");
        });

        modelBuilder.Entity<Keyword>(entity =>
        {
            entity.HasKey(e => e.KeywordId).HasName("PK__KEYWORD__03E8D7CFB32C5755");

            entity.ToTable("KEYWORD");

            entity.Property(e => e.KeywordId)
                .ValueGeneratedOnAdd()
                .HasColumnName("keyword_id");
            entity.Property(e => e.Category)
                .HasMaxLength(255)
                .HasColumnName("category");
            entity.Property(e => e.ImportanceScore)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("importance_score");
            entity.Property(e => e.Keyword1)
                .HasMaxLength(255)
                .HasColumnName("keyword");
            entity.Property(e => e.FieldId).HasColumnName("field_id");
            entity.HasOne(e => e.JobField)
                  .WithMany()
                  .HasForeignKey(e => e.FieldId)
                  .HasConstraintName("FK_KEYWORD_FIELD");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__ROLE__760965CC2B313AA5");

            entity.ToTable("ROLE");

            entity.HasIndex(e => e.RoleName, "UQ__ROLE__783254B19B11D1B3").IsUnique();

            entity.Property(e => e.RoleId)
                .ValueGeneratedOnAdd()
                .HasColumnName("role_id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Description)
                .HasColumnType("text")
                .HasColumnName("description");
            entity.Property(e => e.Permissions)
                .HasColumnType("text")
                .HasColumnName("permissions");
            entity.Property(e => e.RoleName)
                .HasMaxLength(255)
                .HasColumnName("role_name");
        });

        modelBuilder.Entity<SubscriptionPlan>(entity =>
        {
            entity.HasKey(e => e.PlanId).HasName("PK__SUBSCRIP__BE9F8F1D8AFF7171");

            entity.ToTable("SUBSCRIPTION_PLAN");

            entity.Property(e => e.PlanId)
                .ValueGeneratedOnAdd()
                .HasColumnName("plan_id");
            entity.Property(e => e.BillingCycle)
                .HasMaxLength(255)
                .HasColumnName("billing_cycle");
            entity.Property(e => e.Features)
                .HasColumnType("text")
                .HasColumnName("features");
            entity.Property(e => e.IsActive).HasColumnName("is_active");
            entity.Property(e => e.PlanName)
                .HasMaxLength(255)
                .HasColumnName("plan_name");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
            entity.Property(e => e.Price)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("price");
            entity.Property(e => e.UserType)
                .HasMaxLength(255)
                .HasColumnName("user_type");
            entity.Property(e => e.MaxTextInterviewPerDay).HasColumnType("int");
            entity.Property(e => e.MaxVoiceInterviewPerMonth).HasColumnType("int");
            entity.Property(e => e.MaxCvsAllowed).HasColumnType("int");

        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__USER__B9BE370F3A602992");

            entity.ToTable("USER");

            entity.HasIndex(e => e.Email, "UQ__USER__AB6E616416F2222C").IsUnique();

            entity.Property(e => e.UserId)
                .ValueGeneratedOnAdd()
                .HasColumnName("user_id");
            entity.Property(e => e.AccountStatus)
                .HasMaxLength(255)
                .HasColumnName("account_status");
            entity.Property(e => e.GoogleId)
                .HasMaxLength(255)
                .HasColumnName("google_id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .HasColumnName("email");
            entity.Property(e => e.EmailVerified).HasColumnName("email_verified");
            entity.Property(e => e.FirstName)
                .HasMaxLength(255)
                .HasColumnName("first_name");
            entity.Property(e => e.LastLogin)
                .HasColumnType("datetime")
                .HasColumnName("last_login");
            entity.Property(e => e.LastName)
                .HasMaxLength(255)
                .HasColumnName("last_name");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .HasColumnName("password");
            entity.Property(e => e.ProfilePhotoPath)
                .HasMaxLength(255)
                .HasColumnName("profile_photo_path");
            entity.Property(e => e.PhoneNumber)
               .HasMaxLength(255)
               .HasColumnName("phone_number");
            entity.Property(e => e.RegistrationSource)
                .HasMaxLength(255)
                .HasColumnName("registration_source");
            entity.Property(e => e.SubscriptionStatus)
                .HasMaxLength(255)
                .HasColumnName("subscription_status");

        });
        modelBuilder.Entity<CvAnalysisResult>(entity =>
        {
            entity.ToTable("CvAnalysisResult");

            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id)
               .ValueGeneratedOnAdd()
               .HasColumnName("Id");
            entity.Property(e => e.Name).HasMaxLength(200);
            entity.Property(e => e.Email).HasMaxLength(200);
            entity.Property(e => e.Phone).HasMaxLength(50);
            entity.Property(e => e.Summary).HasColumnType("nvarchar(max)");
            entity.Property(e => e.JdAlignment).HasColumnType("nvarchar(max)");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()");
            entity.Property(e => e.MatchScore).HasColumnType("int");
            entity.HasOne(e => e.Cv)
                  .WithMany(r => r.CvAnalysisResults)
                  .HasForeignKey(e => e.CvId)
                  .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(ar => ar.JobDescription)
                  .WithMany(jd => jd.AnalysisResults)
                  .HasForeignKey(ar => ar.JobDescriptionId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // CvSkill
        modelBuilder.Entity<CvSkill>(entity =>
        {
            entity.ToTable("CvSkill");

            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id)
              .ValueGeneratedOnAdd()
              .HasColumnName("Id");
            entity.Property(e => e.SkillName).HasMaxLength(100).IsRequired();
            entity.HasOne(e => e.CvAnalysisResult) // <-- Explicit navigation property
                  .WithMany(r => r.Skills)
                  .HasForeignKey(e => e.CvAnalysisResultId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // CvEducation
        modelBuilder.Entity<CvEducation>(entity =>
        {
            entity.ToTable("CvEducation");

            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id)
              .ValueGeneratedOnAdd()
              .HasColumnName("Id");
            entity.Property(e => e.Degree).HasMaxLength(255);
            entity.Property(e => e.Institution).HasMaxLength(255);
            entity.Property(e => e.Description).HasColumnType("nvarchar(max)");

            entity.HasOne(e => e.CvAnalysisResult)
                   .WithMany(r => r.Education)
                   .HasForeignKey(e => e.CvAnalysisResultId)
                   .OnDelete(DeleteBehavior.Cascade);
        });

        // CvProject
        modelBuilder.Entity<CvProject>(entity =>
        {
            entity.ToTable("CvProject");

            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id)
              .ValueGeneratedOnAdd()
              .HasColumnName("Id");
            entity.Property(e => e.Title).HasMaxLength(255);
            entity.Property(e => e.Description).HasColumnType("nvarchar(max)");

            entity.HasOne(e => e.CvAnalysisResult)
                  .WithMany(r => r.Projects)
                  .HasForeignKey(e => e.CvAnalysisResultId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // CvProjectTechStack
        modelBuilder.Entity<CvProjectTechStack>(entity =>
        {
            entity.ToTable("CvProjectTechStack");

            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id)
              .ValueGeneratedOnAdd()
              .HasColumnName("Id");
            entity.Property(e => e.TechName).HasMaxLength(100);

            entity.HasOne(e => e.CvProject)
                   .WithMany(p => p.TechStacks)
                   .HasForeignKey(e => e.CvProjectId)
                   .OnDelete(DeleteBehavior.Cascade);
        });

        // CvCertification
        modelBuilder.Entity<CvCertification>(entity =>
        {
            entity.ToTable("CvCertification");

            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id)
              .ValueGeneratedOnAdd()
              .HasColumnName("Id");
            entity.Property(e => e.Name).HasMaxLength(255);
            entity.Property(e => e.Issuer).HasMaxLength(255);
            entity.Property(e => e.TimePeriod).HasMaxLength(100);
            entity.Property(e => e.Description).HasColumnType("nvarchar(max)");

            entity.HasOne(e => e.CvAnalysisResult)
                  .WithMany(r => r.Certifications)
                  .HasForeignKey(e => e.CvAnalysisResultId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
        modelBuilder.Entity<UserCharacteristic>(entity =>
        {
            entity.HasKey(e => e.CharacteristicId).HasName("PK__USER_CHA__2EA0BA27A84A9300");

            entity.ToTable("USER_CHARACTERISTICS");

            entity.Property(e => e.CharacteristicId)
                .ValueGeneratedOnAdd()
                .HasColumnName("characteristic_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Data).HasColumnName("data");
            entity.Property(e => e.Metadata).HasColumnName("metadata");
            entity.Property(e => e.Source)
                .HasMaxLength(250)
                .HasColumnName("source");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.UserCharacteristics)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__USER_CHAR__user___236943A5");
        });

        modelBuilder.Entity<UserCharacteristicsHistory>(entity =>
        {
            entity.HasKey(e => e.CharacteristicHistoryId).HasName("PK__USER_CHA__017A68DA4B868630");

            entity.ToTable("USER_CHARACTERISTICS_HISTORY");

            entity.Property(e => e.CharacteristicHistoryId)
                .ValueGeneratedOnAdd()
                .HasColumnName("characteristic_history_id");
            entity.Property(e => e.CharacteristicId).HasColumnName("characteristic_id");
            entity.Property(e => e.Data).HasColumnName("data");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Characteristic).WithMany(p => p.UserCharacteristicsHistories)
                .HasForeignKey(d => d.CharacteristicId)
                .HasConstraintName("FK__USER_CHAR__chara__29221CFB");
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.HasKey(e => e.UserRoleId).HasName("PK__USER_ROL__B8D9ABA2D712A629");

            entity.ToTable("USER_ROLE");

            entity.Property(e => e.UserRoleId)
                .ValueGeneratedOnAdd()
                .HasColumnName("user_role_id");
            entity.Property(e => e.AssignedAt)
                .HasColumnType("datetime")
                .HasColumnName("assigned_at");
            entity.Property(e => e.AssignedBy).HasColumnName("assigned_by");
            entity.Property(e => e.IsPrimary).HasColumnName("is_primary");
            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.AssignedByNavigation).WithMany(p => p.UserRoleAssignedByNavigations)
                .HasForeignKey(d => d.AssignedBy)
                .HasConstraintName("FK__USER_ROLE__assig__07C12930");

            entity.HasOne(d => d.Role).WithMany(p => p.UserRoles)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__USER_ROLE__role___06CD04F7");

            entity.HasOne(d => d.User).WithMany(p => p.UserRoleUsers)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__USER_ROLE__user___05D8E0BE");
        });

        modelBuilder.Entity<UserSubscription>(entity =>
        {
            entity.HasKey(e => e.SubscriptionId).HasName("PK__USER_SUB__863A7EC1A158128C");

            entity.ToTable("USER_SUBSCRIPTION");

            entity.Property(e => e.SubscriptionId)
                .ValueGeneratedOnAdd()
                .HasColumnName("subscription_id");
            entity.Property(e => e.EndDate)
                .HasColumnType("datetime")
                .HasColumnName("end_date");
            entity.Property(e => e.IsAutoRenew).HasColumnName("is_auto_renew");
            entity.Property(e => e.LastPaymentDate)
                .HasColumnType("datetime")
                .HasColumnName("last_payment_date");
            entity.Property(e => e.PaymentStatus)
                .HasMaxLength(255)
                .HasColumnName("payment_status");
            entity.Property(e => e.PlanId).HasColumnName("plan_id");
            entity.Property(e => e.StartDate)
                .HasColumnType("datetime")
                .HasColumnName("start_date");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.InterviewPerDayRemaining).HasColumnType("int");
            entity.Property(e => e.VoiceInterviewRemaining).HasColumnType("int");
            entity.Property(e => e.CvRemaining).HasColumnType("int");
            entity.Property(e => e.LastQuotaResetDate).HasColumnType("datetime");
            entity.HasOne(d => d.Plan).WithMany(p => p.UserSubscriptions)
                .HasForeignKey(d => d.PlanId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__USER_SUBS__plan___160F4887");

            entity.HasOne(d => d.User).WithMany(p => p.UserSubscriptions)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__USER_SUBS__user___151B244E");
        });
        modelBuilder.Entity<FieldCategory>(entity =>
        {
            entity.ToTable("FIELD_CATEGORY");
            entity.HasKey(e => e.CategoryId);
            entity.Property(e => e.CategoryId).ValueGeneratedOnAdd()
               .HasColumnName("category_id"); 
            entity.Property(e => e.CategoryName).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Description).HasColumnType("text");
            entity.Property(e => e.IconPath).HasMaxLength(255);
            entity.Property(e => e.IsDefault);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("getdate()");
        });

        modelBuilder.Entity<JobField>(entity =>
        {
            entity.ToTable("JOB_FIELD");
            entity.HasKey(e => e.FieldId);
            entity.Property(e => e.FieldId)
               .ValueGeneratedOnAdd()
               .HasColumnName("field_id");
            entity.Property(e => e.FieldName).IsRequired().HasMaxLength(255).HasColumnName("field_name");
            entity.Property(e => e.CategoryId).HasColumnName("category_id");
            entity.Property(e => e.CommonSkills).HasColumnType("text").HasColumnName("common_skills");
            entity.Property(e => e.TypicalKeywords).HasColumnType("text").HasColumnName("typical_keywords");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("getdate()").HasColumnName("created_at");
            entity.Property(e => e.IsActive).HasDefaultValue(true).HasColumnName("is_active");

            entity.HasOne(e => e.Category)
                  .WithMany(c => c.JobFields)
                  .HasForeignKey(e => e.CategoryId)
                  .HasConstraintName("FK_JOB_FIELD_CATEGORY");
        });

        modelBuilder.Entity<UserFieldPreference>(entity =>
        {
            entity.ToTable("USER_FIELD_PREFERENCE");
            entity.HasKey(e => e.PreferenceId);
            entity.Property(e => e.ExperienceLevel).HasMaxLength(50);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("getdate()");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("getdate()");
            entity.Property(e => e.IsActive).HasDefaultValue(true);

            entity.HasOne(e => e.JobField)
                  .WithMany(f => f.UserFieldPreferences)
                  .HasForeignKey(e => e.FieldId)
                  .HasConstraintName("FK_USER_FIELD_PREF_FIELD");

            entity.HasOne(e => e.User)
                  .WithMany(u => u.UserFieldPreferences)
                  .HasForeignKey(e => e.UserId)
                  .HasConstraintName("FK_USER_FIELD_PREF_USER");

            entity.HasIndex(e => new { e.UserId, e.FieldId }).IsUnique()
                  .HasDatabaseName("UQ_USER_FIELD_PREFERENCE");

            entity.HasCheckConstraint("CK_USER_FIELD_PREF_EXPERIENCE",
                "[experience_level] IN ('entry', 'junior', 'mid', 'senior', 'lead', 'executive', 'director')");

            entity.HasCheckConstraint("CK_USER_FIELD_PREF_PRIORITY",
                "[priority_rank] >= 1 AND [priority_rank] <= 10");
        });
        modelBuilder.Entity<VirtualInterview>(entity =>
        {
            entity.HasKey(e => e.InterviewId).HasName("PK__VIRTUAL___141E55527CDCF1E3");

            entity.ToTable("VIRTUAL_INTERVIEW");

            entity.Property(e => e.InterviewId)
                .ValueGeneratedOnAdd()
                .HasColumnName("interview_id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.JobId).HasColumnName("job_id");
            entity.Property(e => e.OverallScore)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("overall_score");
            entity.Property(e => e.RecordingPath)
                .HasMaxLength(255)
                .HasColumnName("recording_path");
            entity.Property(e => e.SessionId)
              .HasMaxLength(255)
              .HasColumnName("session_id");
            entity.Property(e => e.CvContent)
               .HasMaxLength(255)
               .HasColumnName("cv_content");
            entity.Property(e => e.JobDescription)
               .HasMaxLength(255)
               .HasColumnName("job_description");
            entity.Property(e => e.Status)
                .HasMaxLength(255)
                .HasColumnName("status");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Job).WithMany(p => p.VirtualInterviews)
                .HasForeignKey(d => d.JobId)
                .HasConstraintName("FK__VIRTUAL_I__job_i__114A936A");

            entity.HasOne(d => d.User).WithMany(p => p.VirtualInterviews)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__VIRTUAL_I__user___10566F31");
        });
        modelBuilder.Entity<CvAnalysisFile>(entity =>
        {
            entity.ToTable("CvAnalysisFiles");

            entity.HasKey(x => x.Id);
            entity.Property(e => e.Id)
               .ValueGeneratedOnAdd();
            entity.Property(x => x.FileUrl).IsRequired().HasMaxLength(500);
            entity.Property(x => x.PublicId).IsRequired().HasMaxLength(255);
            entity.Property(x => x.FileType).HasMaxLength(50);
            entity.Property(x => x.Category).HasMaxLength(100);
            entity.HasOne(x => x.CvVersion)
                .WithMany()
                .HasForeignKey(x => x.CvVersionId)
                .OnDelete(DeleteBehavior.Cascade);
            
        });
        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
