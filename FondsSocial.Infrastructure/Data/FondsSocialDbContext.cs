using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using FondsSocial.Domain.Common;
using FondsSocial.Domain.Entities;

namespace FondsSocial.Infrastructure.Data
{
    public class FondsSocialDbContext : DbContext
    {
        public FondsSocialDbContext(DbContextOptions<FondsSocialDbContext> options) : base(options) { }

        public DbSet<Societe> Societes { get; set; } = null!;
        public DbSet<Agent> Agents { get; set; } = null!;
        public DbSet<TypeDePret> TypeDePrets { get; set; } = null!;
        public DbSet<PieceJustificativeRequise> PieceJustificativeRequises { get; set; } = null!;
        public DbSet<Demande> Demandes { get; set; } = null!;
        public DbSet<HistoriqueStatutDemande> HistoriqueStatutDemandes { get; set; } = null!;
        public DbSet<PieceJustificative> PieceJustificatives { get; set; } = null!;
        public DbSet<MembreComite> MembreComites { get; set; } = null!;
        public DbSet<SeanceComite> SeanceComites { get; set; } = null!;
        public DbSet<ParticipationSeance> ParticipationSeances { get; set; } = null!;
        public DbSet<Decision> Decisions { get; set; } = null!;
        public DbSet<Contrat> Contrats { get; set; } = null!;
        public DbSet<Garantie> Garanties { get; set; } = null!;
        public DbSet<Echeance> Echeances { get; set; } = null!;
        public DbSet<RetenueMensuelle> RetenuesMensuelles { get; set; } = null!;
        public DbSet<BudgetFonds> BudgetsFonds { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(FondsSocialDbContext).Assembly);

            // Global query filter pour soft-delete
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
                {
                    var method = typeof(FondsSocialDbContext).GetMethod(nameof(ApplyIsDeletedFilter), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)!.MakeGenericMethod(entityType.ClrType);
                    method.Invoke(null, new object[] { modelBuilder });
                }
            }

            base.OnModelCreating(modelBuilder);
        }

        private static void ApplyIsDeletedFilter<TEntity>(ModelBuilder builder) where TEntity : BaseEntity
        {
            builder.Entity<TEntity>().HasQueryFilter(e => !e.IsDeleted);
        }

        public override int SaveChanges()
        {
            ApplyAudits();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            ApplyAudits();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void ApplyAudits()
        {
            var now = DateTime.UtcNow;
            var entries = ChangeTracker.Entries<BaseEntity>();
            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedAt = now;
                }
                if (entry.State == EntityState.Modified)
                {
                    entry.Entity.UpdatedAt = now;
                }
            }
        }
    }
}
