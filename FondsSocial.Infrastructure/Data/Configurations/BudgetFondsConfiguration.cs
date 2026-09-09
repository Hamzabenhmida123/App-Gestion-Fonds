using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FondsSocial.Domain.Entities;

namespace FondsSocial.Infrastructure.Data.Configurations
{
    public class BudgetFondsConfiguration : IEntityTypeConfiguration<BudgetFonds>
    {
        public void Configure(EntityTypeBuilder<BudgetFonds> builder)
        {
            builder.ToTable("BudgetFonds");
            builder.HasKey(b => b.Id);
            builder.Property(b => b.Ressources).HasColumnType("decimal(18,3)");
            builder.Property(b => b.Emplois).HasColumnType("decimal(18,3)");
            builder.Property(b => b.Solde).HasColumnType("decimal(18,3)");
            builder.HasIndex(b => new { b.SocieteId, b.Exercice, b.Categorie }).IsUnique().HasDatabaseName("IX_BudgetFonds_Societe_Exercice_Categorie").HasFilter("[IsDeleted] = 0");
            builder.HasOne(b => b.Societe).WithMany(s => s.BudgetFonds).HasForeignKey(b => b.SocieteId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
