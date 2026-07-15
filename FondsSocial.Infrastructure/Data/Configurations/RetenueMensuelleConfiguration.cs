using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FondsSocial.Domain.Entities;

namespace FondsSocial.Infrastructure.Data.Configurations
{
    public class RetenueMensuelleConfiguration : IEntityTypeConfiguration<RetenueMensuelle>
    {
        public void Configure(EntityTypeBuilder<RetenueMensuelle> builder)
        {
            builder.ToTable("RetenuesMensuelles");
            builder.HasKey(r => r.Id);
            builder.Property(r => r.MontantARetenir).HasColumnType("decimal(18,3)");
            builder.Property(r => r.MontantEffectivementRetenu).HasColumnType("decimal(18,3)");
            builder.HasIndex(r => new { r.ContratId, r.Mois }).IsUnique().HasDatabaseName("IX_Retenue_Contrat_Mois");
            builder.HasOne(r => r.Agent).WithMany(a => a.RetenuesMensuelles).HasForeignKey(r => r.AgentId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(r => r.Contrat).WithMany(c => c.RetenuesMensuelles).HasForeignKey(r => r.ContratId).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
