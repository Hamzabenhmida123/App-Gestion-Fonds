using System.Reflection;
using AutoMapper;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace FondsSocial.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            // AutoMapper
            services.AddAutoMapper(Assembly.GetExecutingAssembly());

            // FluentValidation: register validators from assembly
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            // Services
            services.AddScoped<Services.ITypeDePretService, Services.TypeDePretService>();
            services.AddScoped<Services.IAgentService, Services.AgentService>();
            services.AddScoped<Services.IDemandeService, Services.DemandeService>();

            return services;
        }
    }
}
