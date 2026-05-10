using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using QuizGame.DataAccess.Services;
using QuizGame.DataAccess.Models;

namespace QuizGame.DataAccess;

public static class DependencyInjection
{
    public static IServiceCollection AddDataAccess(this IServiceCollection services, IConfiguration config)
    {
        var connectionString = config.GetConnectionString("DefaultConnection");
        services.AddDbContext<QuizGameDbContext>(options => options
            .UseSqlServer(connectionString)
            .UseLazyLoadingProxies()
        );
        
        services.AddScoped<IQuizService, QuizService>();
        services.AddScoped<IQuestionService, QuestionService>();
        services.AddScoped<IQuizSessionService, QuizSessionService>();
        services.AddScoped<IParticipantService, ParticipantService>();
        services.AddScoped<ISubmissionService, SubmissionService>();
        services.AddScoped<IUsersService, UsersService>();
        services.AddIdentity<User, UserRole>(options =>
            {
                // Password settings.
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 6;

                // Lockout settings.
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;
                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<QuizGameDbContext>()
            .AddDefaultTokenProviders();

        return services;
    }
}