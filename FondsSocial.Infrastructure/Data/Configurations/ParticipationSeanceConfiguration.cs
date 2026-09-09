using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FondsSocial.Domain.Entities;

namespace FondsSocial.Infrastructure.Data.Configurations
{
    public class ParticipationSeanceConfiguration : IEntityTypeConfiguration<ParticipationSeance>
    {
        public void Configure(EntityTypeBuilder<ParticipationSeance> builder)
        {
            builder.ToTable("ParticipationSeances");
            builder.HasKey(p => p.Id);
            builder.HasIndex(p => new { p.SeanceComiteId, p.MembreComiteId }).IsUnique().HasDatabaseName("IX_ParticipationSeance_Seance_Membre").HasFilter("[IsDeleted] = 0");
            builder.HasOne(p => p.MembreComite).WithMany(m => m.Participations).HasForeignKey(p => p.MembreComiteId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
