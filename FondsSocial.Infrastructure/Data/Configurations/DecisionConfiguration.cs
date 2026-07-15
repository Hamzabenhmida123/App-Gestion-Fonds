using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FondsSocial.Domain.Entities;

namespace FondsSocial.Infrastructure.Data.Configurations
{
    public class DecisionConfiguration : IEntityTypeConfiguration<Decision>
    {
        public void Configure(EntityTypeBuilder<Decision> builder)
        {
            builder.ToTable("Decisions");
            builder.HasKey(d => d.Id);
            builder.Property(d => d.MontantAccorde).HasColumnType("decimal(18,3)");
            builder.HasOne(d => d.Demande).WithMany(dm => dm.Decisions).HasForeignKey(d => d.DemandeId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(d => d.SeanceComite).WithMany(s => s.Decisions).HasForeignKey(d => d.SeanceComiteId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(d => d.Contrat).WithOne(c => c.Decision).HasForeignKey<Contrat>(c => c.DecisionId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
