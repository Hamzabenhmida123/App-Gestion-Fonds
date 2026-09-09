using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FondsSocial.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FilterUniqueIndexesOnIsDeleted : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TypeDePret_Code",
                table: "TypeDePrets");

            migrationBuilder.DropIndex(
                name: "IX_Societe_Code",
                table: "Societes");

            migrationBuilder.DropIndex(
                name: "IX_Retenue_Contrat_Mois",
                table: "RetenuesMensuelles");

            migrationBuilder.DropIndex(
                name: "IX_ParticipationSeance_Seance_Membre",
                table: "ParticipationSeances");

            migrationBuilder.DropIndex(
                name: "IX_Echeance_Contrat_Numero",
                table: "Echeances");

            migrationBuilder.DropIndex(
                name: "IX_Demande_NumeroDossier",
                table: "Demandes");

            migrationBuilder.DropIndex(
                name: "IX_BudgetFonds_Societe_Exercice_Categorie",
                table: "BudgetFonds");

            migrationBuilder.DropIndex(
                name: "IX_Agent_CIN",
                table: "Agents");

            migrationBuilder.DropIndex(
                name: "IX_Agent_IdentifiantUnique",
                table: "Agents");

            migrationBuilder.DropIndex(
                name: "IX_Agent_Matricule",
                table: "Agents");

            migrationBuilder.CreateIndex(
                name: "IX_TypeDePret_Code",
                table: "TypeDePrets",
                column: "Code",
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_Societe_Code",
                table: "Societes",
                column: "Code",
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_Retenue_Contrat_Mois",
                table: "RetenuesMensuelles",
                columns: new[] { "ContratId", "Mois" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_ParticipationSeance_Seance_Membre",
                table: "ParticipationSeances",
                columns: new[] { "SeanceComiteId", "MembreComiteId" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_Echeance_Contrat_Numero",
                table: "Echeances",
                columns: new[] { "ContratId", "NumeroEcheance" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_Demande_NumeroDossier",
                table: "Demandes",
                column: "NumeroDossier",
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetFonds_Societe_Exercice_Categorie",
                table: "BudgetFonds",
                columns: new[] { "SocieteId", "Exercice", "Categorie" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_Agent_CIN",
                table: "Agents",
                column: "CIN",
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_Agent_IdentifiantUnique",
                table: "Agents",
                column: "IdentifiantUnique",
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_Agent_Matricule",
                table: "Agents",
                column: "Matricule",
                unique: true,
                filter: "[IsDeleted] = 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TypeDePret_Code",
                table: "TypeDePrets");

            migrationBuilder.DropIndex(
                name: "IX_Societe_Code",
                table: "Societes");

            migrationBuilder.DropIndex(
                name: "IX_Retenue_Contrat_Mois",
                table: "RetenuesMensuelles");

            migrationBuilder.DropIndex(
                name: "IX_ParticipationSeance_Seance_Membre",
                table: "ParticipationSeances");

            migrationBuilder.DropIndex(
                name: "IX_Echeance_Contrat_Numero",
                table: "Echeances");

            migrationBuilder.DropIndex(
                name: "IX_Demande_NumeroDossier",
                table: "Demandes");

            migrationBuilder.DropIndex(
                name: "IX_BudgetFonds_Societe_Exercice_Categorie",
                table: "BudgetFonds");

            migrationBuilder.DropIndex(
                name: "IX_Agent_CIN",
                table: "Agents");

            migrationBuilder.DropIndex(
                name: "IX_Agent_IdentifiantUnique",
                table: "Agents");

            migrationBuilder.DropIndex(
                name: "IX_Agent_Matricule",
                table: "Agents");

            migrationBuilder.CreateIndex(
                name: "IX_TypeDePret_Code",
                table: "TypeDePrets",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Societe_Code",
                table: "Societes",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Retenue_Contrat_Mois",
                table: "RetenuesMensuelles",
                columns: new[] { "ContratId", "Mois" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ParticipationSeance_Seance_Membre",
                table: "ParticipationSeances",
                columns: new[] { "SeanceComiteId", "MembreComiteId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Echeance_Contrat_Numero",
                table: "Echeances",
                columns: new[] { "ContratId", "NumeroEcheance" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Demande_NumeroDossier",
                table: "Demandes",
                column: "NumeroDossier",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BudgetFonds_Societe_Exercice_Categorie",
                table: "BudgetFonds",
                columns: new[] { "SocieteId", "Exercice", "Categorie" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Agent_CIN",
                table: "Agents",
                column: "CIN",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Agent_IdentifiantUnique",
                table: "Agents",
                column: "IdentifiantUnique",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Agent_Matricule",
                table: "Agents",
                column: "Matricule",
                unique: true);
        }
    }
}
