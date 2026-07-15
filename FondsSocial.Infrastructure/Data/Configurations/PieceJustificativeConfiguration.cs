using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FondsSocial.Domain.Entities;

namespace FondsSocial.Infrastructure.Data.Configurations
{
    public class PieceJustificativeConfiguration : IEntityTypeConfiguration<PieceJustificative>
    {
        public void Configure(EntityTypeBuilder<PieceJustificative> builder)
        {
            builder.ToTable("PieceJustificatives");
            builder.HasKey(p => p.Id);
            builder.Property(p => p.TypePiece).IsRequired().HasMaxLength(200);
            builder.Property(p => p.CheminFichier).IsRequired().HasMaxLength(1000);
            builder.HasOne(p => p.Demande).WithMany(d => d.PieceJustificatives).HasForeignKey(p => p.DemandeId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
