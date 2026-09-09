using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FondsSocial.Domain.Entities;

namespace FondsSocial.Infrastructure.Data.Configurations
{
    public class SocieteConfiguration : IEntityTypeConfiguration<Societe>
    {
        public void Configure(EntityTypeBuilder<Societe> builder)
        {
            builder.ToTable("Societes");
            builder.HasKey(s => s.Id);
            builder.Property(s => s.Code).IsRequired().HasMaxLength(50);
            builder.HasIndex(s => s.Code).IsUnique().HasDatabaseName("IX_Societe_Code").HasFilter("[IsDeleted] = 0");
            builder.Property(s => s.RaisonSociale).IsRequired().HasMaxLength(250);
            builder.Property(s => s.ReferentielReglesGestion).HasMaxLength(2000);

            builder.HasMany(s => s.Agents).WithOne(a => a.Societe).HasForeignKey(a => a.SocieteId).OnDelete(DeleteBehavior.Restrict);
            builder.HasMany(s => s.BudgetFonds).WithOne(b => b.Societe).HasForeignKey(b => b.SocieteId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
