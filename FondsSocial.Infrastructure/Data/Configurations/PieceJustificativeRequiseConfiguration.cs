using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FondsSocial.Domain.Entities;

namespace FondsSocial.Infrastructure.Data.Configurations
{
    public class PieceJustificativeRequiseConfiguration : IEntityTypeConfiguration<PieceJustificativeRequise>
    {
        public void Configure(EntityTypeBuilder<PieceJustificativeRequise> builder)
        {
            builder.ToTable("PieceJustificativeRequises");
            builder.HasKey(p => p.Id);
            builder.Property(p => p.LibellePiece).IsRequired().HasMaxLength(200);
            builder.HasOne(p => p.TypeDePret).WithMany(t => t.PiecesJustificativesRequises).HasForeignKey(p => p.TypeDePretId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
