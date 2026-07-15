using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FondsSocial.Domain.Entities;

namespace FondsSocial.Infrastructure.Data.Configurations
{
    public class SeanceComiteConfiguration : IEntityTypeConfiguration<SeanceComite>
    {
        public void Configure(EntityTypeBuilder<SeanceComite> builder)
        {
            builder.ToTable("SeanceComites");
            builder.HasKey(s => s.Id);
            builder.Property(s => s.ProcesVerbal).HasColumnType("nvarchar(max)");
            builder.HasMany(s => s.Participations).WithOne(p => p.SeanceComite).HasForeignKey(p => p.SeanceComiteId).OnDelete(DeleteBehavior.Cascade);
            builder.HasMany(s => s.Decisions).WithOne(d => d.SeanceComite).HasForeignKey(d => d.SeanceComiteId).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
