using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using FondsSocial.Infrastructure.Data;
using FondsSocial.Infrastructure.Repositories;
using FondsSocial.Infrastructure.UnitOfWork;

namespace FondsSocial.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // DbContext registration
            services.AddDbContext<FondsSocialDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            // repositories & unit of work
            services.AddScoped<IUnitOfWork, UnitOfWorkService>();

            return services;
        }
    }
}
