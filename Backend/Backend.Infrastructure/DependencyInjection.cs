using Backend.Domain.Interfaces;
using Backend.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Backend.Infrastructure
{
    static public class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
        {
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ILeadRepository,  LeadRepository>();
            services.AddScoped<ILeadStageRepositry, LeadStageRepository>();
            services.AddScoped<IContactRepository, ContactRepository>();
            return services;
        }

    }
}
