using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FondsSocial.Domain.Entities;

namespace FondsSocial.Infrastructure.Data.Configurations
{
    public class MembreComiteConfiguration : IEntityTypeConfiguration<MembreComite>
    {
        public void Configure(EntityTypeBuilder<MembreComite> builder)
        {
            builder.ToTable("MembreComites");
            builder.HasKey(m => m.Id);
            builder.Property(m => m.Nom).IsRequired().HasMaxLength(150);
            builder.Property(m => m.Prenom).IsRequired().HasMaxLength(150);
            builder.Property(m => m.Fonction).HasMaxLength(200);
        }
    }
}
