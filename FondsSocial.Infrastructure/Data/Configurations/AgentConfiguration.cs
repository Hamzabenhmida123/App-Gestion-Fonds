using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FondsSocial.Domain.Entities;

namespace FondsSocial.Infrastructure.Data.Configurations
{
    public class AgentConfiguration : IEntityTypeConfiguration<Agent>
    {
        public void Configure(EntityTypeBuilder<Agent> builder)
        {
            builder.ToTable("Agents");
            builder.HasKey(a => a.Id);
            builder.Property(a => a.Matricule).IsRequired().HasMaxLength(50);
            // Filtre sur IsDeleted: un agent supprimé (soft delete) ne doit plus bloquer
            // la réutilisation de son matricule/CIN/identifiant par un nouvel enregistrement.
            builder.HasIndex(a => a.Matricule).IsUnique().HasDatabaseName("IX_Agent_Matricule").HasFilter("[IsDeleted] = 0");
            builder.Property(a => a.Nom).IsRequired().HasMaxLength(150);
            builder.Property(a => a.Prenom).IsRequired().HasMaxLength(150);
            builder.Property(a => a.CIN).IsRequired().HasMaxLength(20);
            builder.HasIndex(a => a.CIN).IsUnique().HasDatabaseName("IX_Agent_CIN").HasFilter("[IsDeleted] = 0");
            builder.Property(a => a.IdentifiantUnique).IsRequired().HasMaxLength(100);
            builder.HasIndex(a => a.IdentifiantUnique).IsUnique().HasDatabaseName("IX_Agent_IdentifiantUnique").HasFilter("[IsDeleted] = 0");
            builder.Property(a => a.Adresse).HasMaxLength(500);
            builder.Property(a => a.Telephone).HasMaxLength(20);
            builder.Property(a => a.SalaireMensuel).HasColumnType("decimal(18,3)");

            builder.HasOne(a => a.Societe).WithMany(s => s.Agents).HasForeignKey(a => a.SocieteId).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
