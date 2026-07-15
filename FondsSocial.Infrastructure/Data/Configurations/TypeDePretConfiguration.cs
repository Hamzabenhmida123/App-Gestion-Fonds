using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FondsSocial.Domain.Entities;

namespace FondsSocial.Infrastructure.Data.Configurations
{
    public class TypeDePretConfiguration : IEntityTypeConfiguration<TypeDePret>
    {
        public void Configure(EntityTypeBuilder<TypeDePret> builder)
        {
            builder.ToTable("TypeDePrets");
            builder.HasKey(t => t.Id);
            builder.Property(t => t.Code).IsRequired().HasMaxLength(50);
            builder.HasIndex(t => t.Code).IsUnique().HasDatabaseName("IX_TypeDePret_Code");
            builder.Property(t => t.Libelle).IsRequired().HasMaxLength(200);
            builder.Property(t => t.Plafond).HasColumnType("decimal(18,3)");
            builder.Property(t => t.TauxOuMontantFraisGestion).HasColumnType("decimal(18,3)");
        }
    }
}
