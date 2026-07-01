using BloodDonationSystem.Application.Common.Interfaces;
using BloodDonationSystem.Application.Common.Mappings;
using BloodDonationSystem.Application.DTOs.Hospital;
using BloodDonationSystem.Application.Interfaces.Repositories;
using BloodDonationSystem.Application.Interfaces.Services;
using BloodDonationSystem.Application.Services;
using BloodDonationSystem.Domain.Entities;
using BloodDonationSystem.Infrastructure.BackgroundJobs;
using BloodDonationSystem.Infrastructure.Data;
using BloodDonationSystem.Infrastructure.Repositories;
using BloodDonationSystem.Infrastructure.Services;
using BloodDonationSystem.Infrastructure.UnitOfWork;
using Hangfire;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace BloodDonationSystem.Web.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            // DbContext
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly("BloodDonationSystem.Infrastructure")));

            // Identity
            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 6;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.SignIn.RequireConfirmedEmail = false;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

            // Cookie Auth (MVC)
            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Account/Login";
                options.LogoutPath = "/Account/Logout";
                options.AccessDeniedPath = "/Account/AccessDenied";
                options.ExpireTimeSpan = TimeSpan.FromDays(7);
                options.SlidingExpiration = true;
                options.Cookie.Name = "BloodDonationSystem.Auth";
            });

            // JWT (API)
            services.AddAuthentication()
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = configuration["Jwt:Issuer"],
                        ValidAudience = configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(configuration["Jwt:Key"] ?? "BloodDonationSecretKey2025!"))
                    };
                });

            // AutoMapper
            services.AddAutoMapper(typeof(MappingProfile));

            // Hangfire
            services.AddHangfire(config => config
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseSqlServerStorage(configuration.GetConnectionString("DefaultConnection")));
            services.AddHangfireServer();

            // HttpContextAccessor
            services.AddHttpContextAccessor();

            // Repositories
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IDonorRepository, DonorRepository>();
            services.AddScoped<IBloodRequestRepository, BloodRequestRepository>();
            services.AddScoped<IEmergencyRequestRepository, EmergencyRequestRepository>();
            services.AddScoped<IDonationRepository, DonationRepository>();
            services.AddScoped<IRatingRepository, RatingRepository>();
            services.AddScoped<IHospitalRepository, HospitalRepository>();
            services.AddScoped<IBloodBankRepository, BloodBankRepository>();
            services.AddScoped<IChatRepository, ChatRepository>();
            services.AddScoped<INotificationRepository, NotificationRepository>();
            services.AddScoped<IAchievementRepository, AchievementRepository>();
            services.AddScoped<IBadgeRepository, BadgeRepository>();
            services.AddScoped<IFavoriteDonorRepository, FavoriteDonorRepository>();
            services.AddScoped<ISuccessStoryRepository, SuccessStoryRepository>();
            services.AddScoped<IPointRepository, PointRepository>();
            services.AddScoped<ILoginActivityRepository, LoginActivityRepository>();
            services.AddScoped<IRecentSearchRepository, RecentSearchRepository>();
            services.AddScoped<ILocationDataRepository, LocationDataRepository>();
            services.AddScoped<ILocationDataService, LocationDataService>();

            services.AddScoped<IUserProfileService, UserProfileService>();
            services.AddScoped<IChatService, ChatService>();



            // UnitOfWork
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Application Services
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IDonorService, DonorService>();
            services.AddScoped<IBloodRequestService, BloodRequestService>();
            services.AddScoped<IEmergencyService, EmergencyService>();
            services.AddScoped<IDonationService, DonationService>();
            services.AddScoped<IRatingService, RatingService>();
            services.AddScoped<IHospitalService, HospitalService>();
            services.AddScoped<IBloodBankService, BloodBankService>();
       
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IAchievementService, AchievementService>();
            services.AddScoped<IBadgeService, BadgeService>();
            services.AddScoped<IPointService, PointService>();
            services.AddScoped<IGamificationService, GamificationService>();
        
            services.AddScoped<ISearchService, SearchService>();
            services.AddScoped<IStatisticsService, StatisticsService>();
            services.AddScoped<IAdminService, AdminService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<ISmsService, SmsService>();
            services.AddScoped<IBloodCompatibilityService, BloodCompatibilityService>();
            services.AddScoped<IFavoriteDonorService, FavoriteDonorService>();
            services.AddScoped<ISuccessStoryService, SuccessStoryService>();
       
            services.AddScoped<ICurrentUserService, CurrentUserService>();

            // Background Jobs
            services.AddScoped<DonationReminderJob>();
            services.AddScoped<BirthdayWishJob>();
            services.AddScoped<RequestExpiryJob>();
            services.AddScoped<BloodStockWarningJob>();
            services.AddScoped<MonthlyTopDonorJob>();
            services.AddScoped<DonorAvailabilityJob>();     
            services.AddScoped<SuccessStoryCleanupJob>();

            // HttpClient for SMS
            services.AddHttpClient<ISmsService, SmsService>();

            // SignalR
            services.AddSignalR();

            return services;
        }

        public static void ConfigureHangfireJobs(this IApplicationBuilder app)
        {
            RecurringJob.AddOrUpdate<DonationReminderJob>("donation-reminder",
                job => job.ExecuteAsync(), Cron.Daily(8));

            RecurringJob.AddOrUpdate<BirthdayWishJob>("birthday-wish",
                job => job.ExecuteAsync(), Cron.Daily(9));

            RecurringJob.AddOrUpdate<RequestExpiryJob>("request-expiry",
                job => job.ExecuteAsync(), Cron.Hourly());

            RecurringJob.AddOrUpdate<BloodStockWarningJob>("blood-stock-warning",
                job => job.ExecuteAsync(), Cron.Daily(7));

            RecurringJob.AddOrUpdate<MonthlyTopDonorJob>("monthly-top-donor",
                job => job.ExecuteAsync(), Cron.Monthly());

           
            RecurringJob.AddOrUpdate<DonorAvailabilityJob>("donor-availability-check",
                job => job.ExecuteAsync(), Cron.Daily(0)); 

            RecurringJob.AddOrUpdate<SuccessStoryCleanupJob>("success-story-cleanup",
                job => job.ExecuteAsync(), Cron.Hourly()); 
        }
    }
}