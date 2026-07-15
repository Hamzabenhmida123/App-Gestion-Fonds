using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FondsSocial.Domain.Entities;

namespace FondsSocial.Infrastructure.Data.Configurations
{
    public class EcheanceConfiguration : IEntityTypeConfiguration<Echeance>
    {
        public void Configure(EntityTypeBuilder<Echeance> builder)
        {
            builder.ToTable("Echeances");
            builder.HasKey(e => e.Id);
            builder.Property(e => e.CapitalRestantDu).HasColumnType("decimal(18,3)");
            builder.Property(e => e.Mensualite).HasColumnType("decimal(18,3)");
            builder.Property(e => e.CapitalAmorti).HasColumnType("decimal(18,3)");
            builder.Property(e => e.FraisGestion).HasColumnType("decimal(18,3)");
            builder.Property(e => e.SoldeRestant).HasColumnType("decimal(18,3)");
            builder.HasIndex(e => new { e.ContratId, e.NumeroEcheance }).IsUnique().HasDatabaseName("IX_Echeance_Contrat_Numero");
            builder.HasOne(e => e.Contrat).WithMany(c => c.Echeances).HasForeignKey(e => e.ContratId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
