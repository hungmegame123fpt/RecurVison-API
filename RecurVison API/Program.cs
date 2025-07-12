
using Microsoft.AspNetCore.Authentication.Cookies;
using Repository.Interface;
using Repository;
using Service.Interface;
using Service;
using StackExchange.Redis;
using Microsoft.EntityFrameworkCore;
using BusinessObject;
using BusinessObject.DTO.Email;
using BusinessObject.DTO.Payment;
using Microsoft.Extensions.Options;
using Net.payOS;
using Microsoft.AspNetCore.Authentication.Google;
using System.Text.Json;
using System.Text.Json.Serialization;
using BusinessObject.Entities;

namespace RecurVison_API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddDbContext<RecurVisionV1Context>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultSQLConnection"));
            });
            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
                }); ;
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            // Bind PayOS settings
            builder.Services.Configure<PayOSSettings>(
                builder.Configuration.GetSection("PayOS"));

            // Register PayOS as singleton
            builder.Services.AddSingleton<PayOS>(serviceProvider =>
            {
                var payOSSettings = serviceProvider.GetRequiredService<IOptions<PayOSSettings>>().Value;
                return new PayOS(payOSSettings.ClientId, payOSSettings.ApiKey, payOSSettings.ChecksumKey);
            });

            builder.Services.AddSingleton<IConnectionMultiplexer>(provider =>
            {
                var connectionString = builder.Configuration.GetConnectionString("Redis") ?? "localhost:8082";
                return ConnectionMultiplexer.Connect(connectionString);
            });
            builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
            builder.Services.Configure<IISServerOptions>(options =>
            {
                options.MaxRequestBodySize = int.MaxValue;
            });
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultScheme = "CookieAuth";
                options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
            })
                .AddCookie("CookieAuth", options =>
                {
                    options.LoginPath = "/Auth/login";
                    options.LogoutPath = "/Auth/logout";                  
                    options.ExpireTimeSpan = TimeSpan.FromDays(7);
                    options.SlidingExpiration = true;
                    //For production
                    //options.Cookie.HttpOnly = true;
                    //options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                    //options.Cookie.SameSite = SameSiteMode.Strict;
                    //For testing
                    options.Cookie.HttpOnly = false;
                    options.Cookie.SameSite = SameSiteMode.None; 
                    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                })
                .AddGoogle(options =>
                {
                    options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
                    options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
                    options.Scope.Add("profile");
                    options.Scope.Add("email");

                    options.SaveTokens = true; // Keep access token in the authentication ticket
                });

            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.Events.OnRedirectToLogin = context =>
                {
                    if (context.Request.Path.StartsWithSegments("/api"))
                    {
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        return Task.CompletedTask;
                    }

                    context.Response.Redirect(context.RedirectUri);
                    return Task.CompletedTask;
                };
            });

            builder.Services.AddAuthorization();
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.WithOrigins("http://localhost:3000", "https://recruvision.io.vn") // Adjust for your frontend
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials();
                });
            });
            builder.Services.AddAutoMapper(typeof(MappingConfig));
            builder.Services.AddScoped<IUserSubscriptionRepository, UserSubscriptionRepository>();
            builder.Services.AddScoped<ISubscriptionPlanRepository, SubscriptionPlanRepository>();
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IOTPService, OtpService>();
            builder.Services.AddScoped<IEmailService, EmailService>();
            builder.Services.AddScoped<IFileStorageService, FileStorageService>();
            builder.Services.AddScoped<IDocumentParserService, DocumentParserService>();
            builder.Services.AddScoped<ICVRepository, CVRepository>();
            builder.Services.AddScoped<ICVService, CVService>();
            builder.Services.AddScoped<ISubscriptionPaymentService, SubscriptionPaymentService>();
            builder.Services.AddScoped<ISubscriptionPlanService, SubscriptionPlanService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IUserRoleRepository, UserRoleRepository>();
            builder.Services.AddScoped<IBaseRepository<CvSkill>, BaseRepository<CvSkill>>();
            builder.Services.AddScoped<IBaseRepository<CvEducation>, BaseRepository<CvEducation>>();
            builder.Services.AddScoped<IBaseRepository<CvProject>, BaseRepository<CvProject>>();
            builder.Services.AddScoped<IBaseRepository<CvCertification>, BaseRepository<CvCertification>>();
            builder.Services.AddScoped<IUserRoleService, UserRoleService>();
            builder.Services.AddScoped<IUserSubscriptionService, UserSubscriptionService>();
            builder.Services.AddScoped<IInterviewQuestionService, InterviewQuestionService>();
            builder.Services.AddScoped<IVirtualInterviewService, VirtualInterviewService>();
            builder.Services.AddScoped<IKeywordService, KeywordService>();
            builder.Services.AddScoped<ICvVersionService, CvVersionService>();
            builder.Services.AddScoped<ICvAnalysisResultService, CvAnalysisService>();
            builder.Services.AddScoped<IAIClient, AIClient>();
            builder.Services.AddScoped<IAdminStatisticsService, AdminStatisticsService>();
            builder.Services.AddHttpClient<IAIClient, AIClient>();
            builder.Services.AddHostedService<SubscriptionExpiryService>();
            builder.Services.AddHttpContextAccessor();
            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "RecurVision API V1");
                    c.RoutePrefix = string.Empty; // So Swagger UI loads at root
                });
            }
            app.UseCors("AllowAll");
            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}
