using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace FondsSocial.Infrastructure.Data
{
    public class FondsSocialDbContextFactory : IDesignTimeDbContextFactory<FondsSocialDbContext>
    {
        public FondsSocialDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<FondsSocialDbContext>();

            // 1) Try environment variable (CI / user override)
            var envConn = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection");
            string connection = null;
            if (!string.IsNullOrEmpty(envConn))
            {
                connection = envConn;
            }

            // 2) Otherwise try to find FondsSocial.API/appsettings.Development.json in parent folders
            if (string.IsNullOrEmpty(connection))
            {
                var dir = Directory.GetCurrentDirectory();
                for (int i = 0; i < 6; i++)
                {
                    var candidate = Path.Combine(dir, "FondsSocial.API", "appsettings.Development.json");
                    if (File.Exists(candidate))
                    {
                        var config = new ConfigurationBuilder()
                            .AddJsonFile(candidate, optional: false, reloadOnChange: false)
                            .Build();
                        connection = config.GetConnectionString("DefaultConnection");
                        break;
                    }
                    var parent = Directory.GetParent(dir);
                    if (parent == null) break;
                    dir = parent.FullName;
                }
            }

            // 3) Fallback to LocalDB
            if (string.IsNullOrEmpty(connection))
            {
                connection = "Server=(localdb)\\mssqllocaldb;Database=FondsSocialDb;Trusted_Connection=True;MultipleActiveResultSets=true";
            }

            builder.UseSqlServer(connection, o => o.UseCompatibilityLevel(140));
            return new FondsSocialDbContext(builder.Options);
        }
    }
}
