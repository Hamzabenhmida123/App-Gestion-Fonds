using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FondsSocial.Domain.Entities;

namespace FondsSocial.Infrastructure.Data.Configurations
{
    public class DemandeConfiguration : IEntityTypeConfiguration<Demande>
    {
        public void Configure(EntityTypeBuilder<Demande> builder)
        {
            builder.ToTable("Demandes");
            builder.HasKey(d => d.Id);
            builder.Property(d => d.NumeroDossier).IsRequired().HasMaxLength(100);
            builder.HasIndex(d => d.NumeroDossier).IsUnique().HasDatabaseName("IX_Demande_NumeroDossier");
            builder.Property(d => d.MontantDemande).HasColumnType("decimal(18,3)");
            builder.Property(d => d.ScorePriorite).HasColumnType("decimal(18,3)");

            builder.HasOne(d => d.Agent).WithMany(a => a.Demandes).HasForeignKey(d => d.AgentId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(d => d.TypeDePret).WithMany(t => t.Demandes).HasForeignKey(d => d.TypeDePretId).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
