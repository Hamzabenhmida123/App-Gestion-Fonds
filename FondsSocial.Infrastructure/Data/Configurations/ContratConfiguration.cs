using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FondsSocial.Domain.Entities;

namespace FondsSocial.Infrastructure.Data.Configurations
{
    public class ContratConfiguration : IEntityTypeConfiguration<Contrat>
    {
        public void Configure(EntityTypeBuilder<Contrat> builder)
        {
            builder.ToTable("Contrats");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.MontantPrincipal).HasColumnType("decimal(18,3)");
            builder.Property(c => c.FraisGestion).HasColumnType("decimal(18,3)");
            builder.Property(c => c.MontantTotal).HasColumnType("decimal(18,3)");
            builder.HasOne(c => c.Garantie).WithOne(g => g.Contrat).HasForeignKey<Garantie>(g => g.ContratId).OnDelete(DeleteBehavior.Cascade);
            builder.HasMany(c => c.Echeances).WithOne(e => e.Contrat).HasForeignKey(e => e.ContratId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
