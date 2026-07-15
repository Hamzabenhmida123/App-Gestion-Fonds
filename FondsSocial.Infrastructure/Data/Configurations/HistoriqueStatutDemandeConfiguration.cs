using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FondsSocial.Domain.Entities;

namespace FondsSocial.Infrastructure.Data.Configurations
{
    public class HistoriqueStatutDemandeConfiguration : IEntityTypeConfiguration<HistoriqueStatutDemande>
    {
        public void Configure(EntityTypeBuilder<HistoriqueStatutDemande> builder)
        {
            builder.ToTable("HistoriqueStatutDemandes");
            builder.HasKey(h => h.Id);
            builder.Property(h => h.Auteur).IsRequired().HasMaxLength(200);
            builder.Property(h => h.Commentaire).HasMaxLength(2000);
            builder.HasOne(h => h.Demande).WithMany(d => d.HistoriqueStatuts).HasForeignKey(h => h.DemandeId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
