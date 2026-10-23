using Backend.Application.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Backend.Application
{
    static public class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<AuthService>();
            services.AddScoped<TokenService>();
            services.AddScoped<ILeadService, LeadService>();
            services.AddScoped<IUserManager, UserManager>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<INoteService, NoteService>();
            services.AddScoped<IFollowUpService, FollowUpService>();
            services.AddScoped<IActivityService, ActivityService>();
            services.AddScoped<IAnalyticsService, AnalyticsService>();
            services.AddScoped<IIntelligenceService, IntelligenceService>();
            services.AddScoped<IAuditLogService, AuditLogService>();
            services.AddScoped<ICompanyService, CompanyService>();
            return services;
        }            
    }
}

