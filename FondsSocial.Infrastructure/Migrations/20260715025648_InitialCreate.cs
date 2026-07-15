using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FondsSocial.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MembreComites",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nom = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Prenom = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Fonction = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Actif = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MembreComites", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SeanceComites",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ProcesVerbal = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StatutVisaSignature = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SeanceComites", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Societes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    RaisonSociale = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    ReferentielReglesGestion = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Societes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TypeDePrets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Libelle = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Categorie = table.Column<int>(type: "int", nullable: false),
                    Plafond = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    DureeMaxMois = table.Column<int>(type: "int", nullable: false),
                    FranchiseMois = table.Column<int>(type: "int", nullable: false),
                    ModeCalculFraisGestion = table.Column<int>(type: "int", nullable: false),
                    TauxOuMontantFraisGestion = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    Actif = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TypeDePrets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ParticipationSeances",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SeanceComiteId = table.Column<int>(type: "int", nullable: false),
                    MembreComiteId = table.Column<int>(type: "int", nullable: false),
                    AVise = table.Column<bool>(type: "bit", nullable: false),
                    DateVisa = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParticipationSeances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ParticipationSeances_MembreComites_MembreComiteId",
                        column: x => x.MembreComiteId,
                        principalTable: "MembreComites",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ParticipationSeances_SeanceComites_SeanceComiteId",
                        column: x => x.SeanceComiteId,
                        principalTable: "SeanceComites",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Agents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Matricule = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Nom = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Prenom = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    CIN = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IdentifiantUnique = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Grade = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Fonction = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Adresse = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Telephone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    DateTitularisation = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SituationFamiliale = table.Column<int>(type: "int", nullable: true),
                    NombreEnfantsACharge = table.Column<int>(type: "int", nullable: false),
                    SocieteId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Agents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Agents_Societes_SocieteId",
                        column: x => x.SocieteId,
                        principalTable: "Societes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BudgetFonds",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SocieteId = table.Column<int>(type: "int", nullable: false),
                    Exercice = table.Column<int>(type: "int", nullable: false),
                    Categorie = table.Column<int>(type: "int", nullable: false),
                    Ressources = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    Emplois = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    Solde = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BudgetFonds", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BudgetFonds_Societes_SocieteId",
                        column: x => x.SocieteId,
                        principalTable: "Societes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PieceJustificativeRequises",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TypeDePretId = table.Column<int>(type: "int", nullable: false),
                    LibellePiece = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Obligatoire = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PieceJustificativeRequises", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PieceJustificativeRequises_TypeDePrets_TypeDePretId",
                        column: x => x.TypeDePretId,
                        principalTable: "TypeDePrets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Demandes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NumeroDossier = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    AgentId = table.Column<int>(type: "int", nullable: false),
                    TypeDePretId = table.Column<int>(type: "int", nullable: false),
                    DateDepot = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MontantDemande = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    StatutCourant = table.Column<int>(type: "int", nullable: false),
                    ScorePriorite = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Demandes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Demandes_Agents_AgentId",
                        column: x => x.AgentId,
                        principalTable: "Agents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Demandes_TypeDePrets_TypeDePretId",
                        column: x => x.TypeDePretId,
                        principalTable: "TypeDePrets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Decisions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DemandeId = table.Column<int>(type: "int", nullable: false),
                    SeanceComiteId = table.Column<int>(type: "int", nullable: false),
                    SensDecision = table.Column<int>(type: "int", nullable: false),
                    MontantAccorde = table.Column<decimal>(type: "decimal(18,3)", nullable: true),
                    DateNotification = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DatePeremption = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Decisions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Decisions_Demandes_DemandeId",
                        column: x => x.DemandeId,
                        principalTable: "Demandes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Decisions_SeanceComites_SeanceComiteId",
                        column: x => x.SeanceComiteId,
                        principalTable: "SeanceComites",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HistoriqueStatutDemandes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DemandeId = table.Column<int>(type: "int", nullable: false),
                    Statut = table.Column<int>(type: "int", nullable: false),
                    DateChangement = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Auteur = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Commentaire = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistoriqueStatutDemandes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HistoriqueStatutDemandes_Demandes_DemandeId",
                        column: x => x.DemandeId,
                        principalTable: "Demandes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PieceJustificatives",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DemandeId = table.Column<int>(type: "int", nullable: false),
                    TypePiece = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CheminFichier = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    DateDepot = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StatutVerification = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PieceJustificatives", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PieceJustificatives_Demandes_DemandeId",
                        column: x => x.DemandeId,
                        principalTable: "Demandes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Contrats",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DecisionId = table.Column<int>(type: "int", nullable: false),
                    DateSignature = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MontantPrincipal = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    FraisGestion = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    MontantTotal = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    DureeMois = table.Column<int>(type: "int", nullable: false),
                    DatePremiereEcheance = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contrats", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Contrats_Decisions_DecisionId",
                        column: x => x.DecisionId,
                        principalTable: "Decisions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Echeances",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ContratId = table.Column<int>(type: "int", nullable: false),
                    NumeroEcheance = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CapitalRestantDu = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    Mensualite = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    CapitalAmorti = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    FraisGestion = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    SoldeRestant = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    Statut = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Echeances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Echeances_Contrats_ContratId",
                        column: x => x.ContratId,
                        principalTable: "Contrats",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Garanties",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ContratId = table.Column<int>(type: "int", nullable: false),
                    AssuranceVieSouscrite = table.Column<bool>(type: "bit", nullable: false),
                    DateAssuranceVie = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReferencePoliceAssurance = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    TraiteSigneeLegalisee = table.Column<bool>(type: "bit", nullable: false),
                    DateTraite = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EngagementRemboursementSigne = table.Column<bool>(type: "bit", nullable: false),
                    DateEngagement = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AutorisationRetenueSalaireSignee = table.Column<bool>(type: "bit", nullable: false),
                    DateAutorisationRetenue = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Garanties", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Garanties_Contrats_ContratId",
                        column: x => x.ContratId,
                        principalTable: "Contrats",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RetenuesMensuelles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Mois = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AgentId = table.Column<int>(type: "int", nullable: false),
                    ContratId = table.Column<int>(type: "int", nullable: false),
                    MontantARetenir = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    MontantEffectivementRetenu = table.Column<decimal>(type: "decimal(18,3)", nullable: true),
                    Statut = table.Column<int>(type: "int", nullable: false),
                    ReferencePaie = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RetenuesMensuelles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RetenuesMensuelles_Agents_AgentId",
                        column: x => x.AgentId,
                        principalTable: "Agents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RetenuesMensuelles_Contrats_ContratId",
                        column: x => x.ContratId,
                        principalTable: "Contrats",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

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

            migrationBuilder.CreateIndex(
                name: "IX_Agents_SocieteId",
                table: "Agents",
                column: "SocieteId");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetFonds_Societe_Exercice_Categorie",
                table: "BudgetFonds",
                columns: new[] { "SocieteId", "Exercice", "Categorie" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Contrats_DecisionId",
                table: "Contrats",
                column: "DecisionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Decisions_DemandeId",
                table: "Decisions",
                column: "DemandeId");

            migrationBuilder.CreateIndex(
                name: "IX_Decisions_SeanceComiteId",
                table: "Decisions",
                column: "SeanceComiteId");

            migrationBuilder.CreateIndex(
                name: "IX_Demande_NumeroDossier",
                table: "Demandes",
                column: "NumeroDossier",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Demandes_AgentId",
                table: "Demandes",
                column: "AgentId");

            migrationBuilder.CreateIndex(
                name: "IX_Demandes_TypeDePretId",
                table: "Demandes",
                column: "TypeDePretId");

            migrationBuilder.CreateIndex(
                name: "IX_Echeance_Contrat_Numero",
                table: "Echeances",
                columns: new[] { "ContratId", "NumeroEcheance" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Garanties_ContratId",
                table: "Garanties",
                column: "ContratId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HistoriqueStatutDemandes_DemandeId",
                table: "HistoriqueStatutDemandes",
                column: "DemandeId");

            migrationBuilder.CreateIndex(
                name: "IX_ParticipationSeance_Seance_Membre",
                table: "ParticipationSeances",
                columns: new[] { "SeanceComiteId", "MembreComiteId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ParticipationSeances_MembreComiteId",
                table: "ParticipationSeances",
                column: "MembreComiteId");

            migrationBuilder.CreateIndex(
                name: "IX_PieceJustificativeRequises_TypeDePretId",
                table: "PieceJustificativeRequises",
                column: "TypeDePretId");

            migrationBuilder.CreateIndex(
                name: "IX_PieceJustificatives_DemandeId",
                table: "PieceJustificatives",
                column: "DemandeId");

            migrationBuilder.CreateIndex(
                name: "IX_Retenue_Contrat_Mois",
                table: "RetenuesMensuelles",
                columns: new[] { "ContratId", "Mois" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RetenuesMensuelles_AgentId",
                table: "RetenuesMensuelles",
                column: "AgentId");

            migrationBuilder.CreateIndex(
                name: "IX_Societe_Code",
                table: "Societes",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TypeDePret_Code",
                table: "TypeDePrets",
                column: "Code",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BudgetFonds");

            migrationBuilder.DropTable(
                name: "Echeances");

            migrationBuilder.DropTable(
                name: "Garanties");

            migrationBuilder.DropTable(
                name: "HistoriqueStatutDemandes");

            migrationBuilder.DropTable(
                name: "ParticipationSeances");

            migrationBuilder.DropTable(
                name: "PieceJustificativeRequises");

            migrationBuilder.DropTable(
                name: "PieceJustificatives");

            migrationBuilder.DropTable(
                name: "RetenuesMensuelles");

            migrationBuilder.DropTable(
                name: "MembreComites");

            migrationBuilder.DropTable(
                name: "Contrats");

            migrationBuilder.DropTable(
                name: "Decisions");

            migrationBuilder.DropTable(
                name: "Demandes");

            migrationBuilder.DropTable(
                name: "SeanceComites");

            migrationBuilder.DropTable(
                name: "Agents");

            migrationBuilder.DropTable(
                name: "TypeDePrets");

            migrationBuilder.DropTable(
                name: "Societes");
        }
    }
}
