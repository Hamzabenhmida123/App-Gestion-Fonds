using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace FondsSocial.Infrastructure.Data
{
    public class FondsSocialDbContextFactory : IDesignTimeDbContextFactory<FondsSocialDbContext>
    {
        public FondsSocialDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<FondsSocialDbContext>();
            // Chaîne de connexion minimale pour les outils de migration. L'utilisateur peut la remplacer via --connection
            // Utiliser l'instance SQL Server locale (ex. HAMZA\\SQLEXPRESS) comme demandé
            var connection = "Server=HAMZA\\SQLEXPRESS;Database=FondsSocialDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True";
            builder.UseSqlServer(connection, o => o.UseCompatibilityLevel(140));
            return new FondsSocialDbContext(builder.Options);
        }
    }
}
