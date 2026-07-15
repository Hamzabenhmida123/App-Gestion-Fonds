using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FondsSocial.Domain.Entities;

namespace FondsSocial.Infrastructure.Data.Configurations
{
    public class GarantieConfiguration : IEntityTypeConfiguration<Garantie>
    {
        public void Configure(EntityTypeBuilder<Garantie> builder)
        {
            builder.ToTable("Garanties");
            builder.HasKey(g => g.Id);
            builder.Property(g => g.ReferencePoliceAssurance).HasMaxLength(200);
        }
    }
}
