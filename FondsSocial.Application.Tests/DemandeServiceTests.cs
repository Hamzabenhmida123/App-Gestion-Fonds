#nullable enable

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;
using Moq;
using Xunit;
using FondsSocial.Application.Services;
using FondsSocial.Application.DTOs;
using FondsSocial.Domain.Entities;
using FondsSocial.Infrastructure.UnitOfWork;
using FondsSocial.Infrastructure.Repositories;
using AutoMapper;
using FondsSocial.Application.Mapping;
using FondsSocial.Domain.Enums;

namespace FondsSocial.Application.Tests
{
    public class DemandeServiceTests
    {
        private readonly IMapper _mapper;

        public DemandeServiceTests()
        {
            var cfg = new MapperConfiguration(cfg => cfg.AddProfile(new MappingProfile()));
            _mapper = cfg.CreateMapper();
        }

        private static Agent CreateEligibleAgent(int id = 1, decimal? salaire = 5000m, int? dureeAnciennete = 2)
        {
            return new Agent
            {
                Id = id,
                DateTitularisation = DateTime.UtcNow.AddYears(-(dureeAnciennete ?? 1)) - (dureeAnciennete.HasValue && dureeAnciennete.Value == 0 ? TimeSpan.FromDays(1) : TimeSpan.Zero),
                SalaireMensuel = salaire
            };
        }

        /// <summary>
        /// CreateAsync now opens a serializable transaction around its eligibility checks
        /// (to close a race condition between concurrent requests for the same agent), so
        /// every test needs IUnitOfWork.BeginTransactionAsync to return a working stub
        /// transaction, not just the repositories under test.
        /// </summary>
        private static Mock<IUnitOfWork> CreateMockUow()
        {
            return CreateMockUow(out _);
        }

        /// <summary>
        /// Overload exposing the stub transaction mock so tests can verify Commit/Rollback
        /// were actually called by ExecuteInSerializableTransactionAsync.
        /// </summary>
        private static Mock<IUnitOfWork> CreateMockUow(out Mock<IDbContextTransaction> mockTransaction)
        {
            var mockUow = new Mock<IUnitOfWork>();
            mockTransaction = new Mock<IDbContextTransaction>();
            mockTransaction.Setup(t => t.CommitAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
            mockTransaction.Setup(t => t.RollbackAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
            mockTransaction.Setup(t => t.DisposeAsync()).Returns(ValueTask.CompletedTask);
            mockUow.Setup(u => u.BeginTransactionAsync(It.IsAny<IsolationLevel>())).ReturnsAsync(mockTransaction.Object);
            return mockUow;
        }

        private static Mock<IRepository<T>> StubRepo<T>(T? entity = null, IEnumerable<T>? items = null) where T : class
        {
            var repo = new Mock<IRepository<T>>();
            repo.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(entity);
            repo.Setup(r => r.FindAsync(It.IsAny<Expression<Func<T, bool>>>()))
                .ReturnsAsync((Expression<Func<T, bool>> predicate) =>
                    (items ?? new List<T>()).Where(predicate.Compile()).ToList());
            repo.Setup(r => r.GetAllAsync()).ReturnsAsync(items ?? new List<T>());
            return repo;
        }

        private static CreateDemandeDto CreateDemandeDto(int agentId = 1, int typeId = 1, decimal montant = 1000m)
        {
            return new CreateDemandeDto
            {
                AgentId = agentId,
                TypeDePretId = typeId,
                MontantDemande = montant,
                DateDepot = DateTime.UtcNow
            };
        }

        #region Existing behavior

        [Fact]
        public async Task CreateAsync_Throws_When_Agent_NotFound()
        {
            var mockUow = CreateMockUow();
            mockUow.SetupGet(u => u.Agents).Returns(StubRepo<Agent>().Object);

            var service = new DemandeService(mockUow.Object, _mapper);

            var dto = CreateDemandeDto();

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () => await service.CreateAsync(dto));
            Assert.Equal("Agent introuvable", ex.Message);
        }

        [Fact]
        public async Task CreateAsync_Throws_When_TypeDePret_NotFound()
        {
            var mockUow = CreateMockUow();
            mockUow.SetupGet(u => u.Agents).Returns(StubRepo(CreateEligibleAgent()).Object);
            mockUow.SetupGet(u => u.TypeDePrets).Returns(StubRepo<TypeDePret>().Object);

            var service = new DemandeService(mockUow.Object, _mapper);

            var dto = CreateDemandeDto();

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () => await service.CreateAsync(dto));
            Assert.Equal("Type de prêt introuvable", ex.Message);
        }

        [Fact]
        public async Task CreateAsync_Throws_When_Montant_Exceeds_Plafond()
        {
            var mockUow = CreateMockUow();

            var agent = CreateEligibleAgent();
            var type = new TypeDePret { Id = 1, Plafond = 1000, FranchiseMois = 0, DureeMaxMois = 12 };

            mockUow.SetupGet(u => u.Agents).Returns(StubRepo(agent).Object);
            mockUow.SetupGet(u => u.TypeDePrets).Returns(StubRepo(type).Object);
            mockUow.SetupGet(u => u.Demandes).Returns(StubRepo<Demande>(items: new List<Demande>()).Object);

            var service = new DemandeService(mockUow.Object, _mapper);

            var dto = CreateDemandeDto(montant: 2000);

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () => await service.CreateAsync(dto));
            Assert.Contains("dépasse le plafond", ex.Message);
        }

        [Fact]
        public async Task CreateAsync_Throws_When_Franchise_Not_Respected()
        {
            var mockUow = CreateMockUow();

            var agent = CreateEligibleAgent();
            var type = new TypeDePret { Id = 1, Plafond = 10000, FranchiseMois = 12, DureeMaxMois = 12 };

            var previous = new Demande { Id = 5, AgentId = 1, TypeDePretId = 1, DateDepot = DateTime.UtcNow.AddMonths(-1) };

            mockUow.SetupGet(u => u.Agents).Returns(StubRepo(agent).Object);
            mockUow.SetupGet(u => u.TypeDePrets).Returns(StubRepo(type).Object);
            mockUow.SetupGet(u => u.Demandes).Returns(StubRepo<Demande>(items: new List<Demande> { previous }).Object);

            var service = new DemandeService(mockUow.Object, _mapper);

            var dto = CreateDemandeDto(montant: 1000);

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () => await service.CreateAsync(dto));
            Assert.Contains("Franchise non respectée", ex.Message);
        }

        [Fact]
        public async Task CreateAsync_Franchise_Counts_Full_Months_Not_Calendar_Month_Difference()
        {
            // Régression: avant le correctif, une demande du 31 décembre suivie d'une nouvelle
            // demande le 1er janvier (1 seul jour d'écart) était comptée comme "1 mois écoulé"
            // par le calcul naïf (année*12+mois), ce qui aurait permis de contourner une
            // franchise d'1 mois alors qu'aucun mois plein ne s'est réellement écoulé.
            var mockUow = CreateMockUow();

            var agent = new Agent { Id = 1, DateTitularisation = new DateTime(2000, 1, 1), SalaireMensuel = 5000m };
            var type = new TypeDePret { Id = 1, Plafond = 10000, FranchiseMois = 1, DureeMaxMois = 12 };
            var previous = new Demande { Id = 5, AgentId = 1, TypeDePretId = 1, DateDepot = new DateTime(2025, 12, 31) };

            mockUow.SetupGet(u => u.Agents).Returns(StubRepo(agent).Object);
            mockUow.SetupGet(u => u.TypeDePrets).Returns(StubRepo(type).Object);
            mockUow.SetupGet(u => u.Demandes).Returns(StubRepo<Demande>(items: new List<Demande> { previous }).Object);

            var service = new DemandeService(mockUow.Object, _mapper);
            var dto = new CreateDemandeDto { AgentId = 1, TypeDePretId = 1, MontantDemande = 1000, DateDepot = new DateTime(2026, 1, 1) };

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () => await service.CreateAsync(dto));
            Assert.Contains("Franchise non respectée", ex.Message);
        }

        [Fact]
        public async Task CreateAsync_Rejected_Previous_Demand_Does_Not_Block_Franchise()
        {
            var mockUow = CreateMockUow();

            var agent = CreateEligibleAgent(salaire: 10000m);
            var type = new TypeDePret { Id = 1, Categorie = CategorieBudget.Vehicule, Plafond = 10000, FranchiseMois = 12, DureeMaxMois = 12 };

            // Demande précédente rejetée il y a 1 mois (dans la période de franchise)
            var rejetee = new Demande { Id = 5, AgentId = 1, TypeDePretId = 1, DateDepot = DateTime.UtcNow.AddMonths(-1), StatutCourant = StatutDemande.Rejetee };

            mockUow.SetupGet(u => u.Agents).Returns(StubRepo(agent).Object);
            mockUow.SetupGet(u => u.TypeDePrets).Returns(StubRepo(type, new List<TypeDePret> { type }).Object);
            mockUow.SetupGet(u => u.Demandes).Returns(StubRepo<Demande>(items: new List<Demande> { rejetee }).Object);
            mockUow.SetupGet(u => u.RetenuesMensuelles).Returns(StubRepo<RetenueMensuelle>(items: new List<RetenueMensuelle>()).Object);
            mockUow.SetupGet(u => u.Decisions).Returns(StubRepo<Decision>(items: new List<Decision>()).Object);
            mockUow.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            var service = new DemandeService(mockUow.Object, _mapper);

            var dto = CreateDemandeDto(montant: 1000);

            var result = await service.CreateAsync(dto);

            Assert.NotNull(result);
        }

        [Fact]
        public async Task CreateAsync_Caduque_Previous_Demand_Does_Not_Block_Franchise()
        {
            var mockUow = CreateMockUow();

            var agent = CreateEligibleAgent(salaire: 10000m);
            var type = new TypeDePret { Id = 1, Categorie = CategorieBudget.Vehicule, Plafond = 10000, FranchiseMois = 12, DureeMaxMois = 12 };

            // Demande précédente caduque il y a 1 mois (dans la période de franchise)
            var caduque = new Demande { Id = 5, AgentId = 1, TypeDePretId = 1, DateDepot = DateTime.UtcNow.AddMonths(-1), StatutCourant = StatutDemande.Caduque };

            mockUow.SetupGet(u => u.Agents).Returns(StubRepo(agent).Object);
            mockUow.SetupGet(u => u.TypeDePrets).Returns(StubRepo(type, new List<TypeDePret> { type }).Object);
            mockUow.SetupGet(u => u.Demandes).Returns(StubRepo<Demande>(items: new List<Demande> { caduque }).Object);
            mockUow.SetupGet(u => u.RetenuesMensuelles).Returns(StubRepo<RetenueMensuelle>(items: new List<RetenueMensuelle>()).Object);
            mockUow.SetupGet(u => u.Decisions).Returns(StubRepo<Decision>(items: new List<Decision>()).Object);
            mockUow.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            var service = new DemandeService(mockUow.Object, _mapper);

            var dto = CreateDemandeDto(montant: 1000);

            var result = await service.CreateAsync(dto);

            Assert.NotNull(result);
        }

        #region Clôture du dépôt

        [Fact]
        public async Task CloturerDepotAsync_Returns_False_When_Demande_NotFound()
        {
            var mockUow = CreateMockUow();
            mockUow.SetupGet(u => u.Demandes).Returns(StubRepo<Demande>().Object);

            var service = new DemandeService(mockUow.Object, _mapper);

            var ok = await service.CloturerDepotAsync(999, "tester", "clôture");

            Assert.False(ok);
        }

        [Fact]
        public async Task CloturerDepotAsync_Throws_When_Required_Pieces_Missing()
        {
            var mockUow = CreateMockUow();

            var demande = new Demande { Id = 1, TypeDePretId = 1, StatutCourant = StatutDemande.Deposee };
            var requise = new PieceJustificativeRequise { Id = 1, TypeDePretId = 1, LibellePiece = "CIN", Obligatoire = true };

            mockUow.SetupGet(u => u.Demandes).Returns(StubRepo(demande).Object);
            mockUow.SetupGet(u => u.PieceJustificativeRequises).Returns(StubRepo<PieceJustificativeRequise>(items: new List<PieceJustificativeRequise> { requise }).Object);
            mockUow.SetupGet(u => u.PieceJustificatives).Returns(StubRepo<PieceJustificative>(items: new List<PieceJustificative>()).Object);

            var service = new DemandeService(mockUow.Object, _mapper);

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () => await service.CloturerDepotAsync(1, "tester", "clôture"));
            Assert.Contains("Dossier incomplet", ex.Message);
            Assert.Contains("CIN", ex.Message);
        }

        [Fact]
        public async Task CloturerDepotAsync_Throws_When_Pieces_Non_Conformes()
        {
            var mockUow = CreateMockUow();

            var demande = new Demande { Id = 1, TypeDePretId = 1, StatutCourant = StatutDemande.Deposee };
            var requise = new PieceJustificativeRequise { Id = 1, TypeDePretId = 1, LibellePiece = "CIN", Obligatoire = true };
            var piece = new PieceJustificative { Id = 1, DemandeId = 1, TypePiece = "CIN", StatutVerification = StatutVerification.NonConforme };

            mockUow.SetupGet(u => u.Demandes).Returns(StubRepo(demande).Object);
            mockUow.SetupGet(u => u.PieceJustificativeRequises).Returns(StubRepo<PieceJustificativeRequise>(items: new List<PieceJustificativeRequise> { requise }).Object);
            mockUow.SetupGet(u => u.PieceJustificatives).Returns(StubRepo<PieceJustificative>(items: new List<PieceJustificative> { piece }).Object);

            var service = new DemandeService(mockUow.Object, _mapper);

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () => await service.CloturerDepotAsync(1, "tester", "clôture"));
            Assert.Contains("pièces non conformes", ex.Message);
            Assert.Contains("CIN", ex.Message);
        }

        [Fact]
        public async Task CloturerDepotAsync_Throws_When_Required_Piece_Still_EnAttente()
        {
            // Régression: une pièce jamais vérifiée (EnAttente) ne doit pas suffire à clôturer
            // le dépôt - il ne suffit pas qu'elle soit "pas NonConforme", RH doit l'avoir
            // explicitement validée comme Conforme.
            var mockUow = CreateMockUow();

            var demande = new Demande { Id = 1, TypeDePretId = 1, StatutCourant = StatutDemande.Deposee };
            var requise = new PieceJustificativeRequise { Id = 1, TypeDePretId = 1, LibellePiece = "CIN", Obligatoire = true };
            var piece = new PieceJustificative { Id = 1, DemandeId = 1, TypePiece = "CIN", StatutVerification = StatutVerification.EnAttente };

            mockUow.SetupGet(u => u.Demandes).Returns(StubRepo(demande).Object);
            mockUow.SetupGet(u => u.PieceJustificativeRequises).Returns(StubRepo<PieceJustificativeRequise>(items: new List<PieceJustificativeRequise> { requise }).Object);
            mockUow.SetupGet(u => u.PieceJustificatives).Returns(StubRepo<PieceJustificative>(items: new List<PieceJustificative> { piece }).Object);

            var service = new DemandeService(mockUow.Object, _mapper);

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () => await service.CloturerDepotAsync(1, "tester", "clôture"));
            Assert.Contains("non encore vérifiées", ex.Message);
            Assert.Contains("CIN", ex.Message);
        }

        [Fact]
        public async Task CloturerDepotAsync_Succeeds_And_Registers_Demande()
        {
            var mockUow = CreateMockUow();

            var demande = new Demande { Id = 1, TypeDePretId = 1, StatutCourant = StatutDemande.Deposee };
            var requise = new PieceJustificativeRequise { Id = 1, TypeDePretId = 1, LibellePiece = "CIN", Obligatoire = true };
            var piece = new PieceJustificative { Id = 1, DemandeId = 1, TypePiece = "CIN", StatutVerification = StatutVerification.Conforme };

            var mockDemandes = new Mock<IRepository<Demande>>();
            mockDemandes.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(demande);
            mockDemandes.Setup(r => r.Update(It.IsAny<Demande>()));
            mockUow.SetupGet(u => u.Demandes).Returns(mockDemandes.Object);
            mockUow.SetupGet(u => u.PieceJustificativeRequises).Returns(StubRepo<PieceJustificativeRequise>(items: new List<PieceJustificativeRequise> { requise }).Object);
            mockUow.SetupGet(u => u.PieceJustificatives).Returns(StubRepo<PieceJustificative>(items: new List<PieceJustificative> { piece }).Object);
            mockUow.SetupGet(u => u.HistoriqueStatutDemandes).Returns(StubRepo<HistoriqueStatutDemande>(items: new List<HistoriqueStatutDemande>()).Object);
            mockUow.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            var service = new DemandeService(mockUow.Object, _mapper);

            var ok = await service.CloturerDepotAsync(1, "tester", "clôture");

            Assert.True(ok);
            Assert.Equal(StatutDemande.Enregistree, demande.StatutCourant);
            mockDemandes.Verify(m => m.Update(It.IsAny<Demande>()), Times.Once);
            mockUow.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task CloturerDepotAsync_Throws_When_Demande_Not_Deposee()
        {
            var mockUow = CreateMockUow();

            var demande = new Demande { Id = 1, TypeDePretId = 1, StatutCourant = StatutDemande.AEtude };
            mockUow.SetupGet(u => u.Demandes).Returns(StubRepo(demande).Object);

            var service = new DemandeService(mockUow.Object, _mapper);

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () => await service.CloturerDepotAsync(1, "tester", "clôture"));
            Assert.Contains("Clôture impossible", ex.Message);
        }

        #endregion

        [Fact]
        public async Task TransitionStatutAsync_Allows_Valid_Transition()
        {
            var mockUow = CreateMockUow();

            var demande = new Demande { Id = 10, StatutCourant = StatutDemande.Deposee };

            var mockDemandes = new Mock<IRepository<Demande>>();
            mockDemandes.Setup(r => r.GetByIdAsync(10)).ReturnsAsync(demande);
            mockDemandes.Setup(r => r.Update(It.IsAny<Demande>()));
            mockUow.SetupGet(u => u.Demandes).Returns(mockDemandes.Object);

            var mockHist = new Mock<IRepository<HistoriqueStatutDemande>>();
            mockHist.Setup(r => r.AddAsync(It.IsAny<HistoriqueStatutDemande>())).Returns(Task.CompletedTask);
            mockUow.SetupGet(u => u.HistoriqueStatutDemandes).Returns(mockHist.Object);

            mockUow.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            var service = new DemandeService(mockUow.Object, _mapper);

            var ok = await service.TransitionStatutAsync(10, StatutDemande.Enregistree, "tester", "ok");

            Assert.True(ok);
            mockDemandes.Verify(m => m.Update(It.IsAny<Demande>()), Times.Once);
            mockUow.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task ChangePieceStatusAsync_Writes_Actual_Demande_Status_Not_Hardcoded_AEtude()
        {
            // Régression: la méthode écrivait auparavant Statut = AEtude en dur dans l'historique,
            // quel que soit le statut réel de la demande (elle ne la fait pourtant pas transitionner).
            // Ici la demande est à Deposee: l'historique doit refléter Deposee, pas AEtude.
            var mockUow = CreateMockUow();

            var piece = new PieceJustificative { Id = 1, DemandeId = 7, StatutVerification = StatutVerification.EnAttente };
            var demande = new Demande { Id = 7, StatutCourant = StatutDemande.Deposee };

            mockUow.SetupGet(u => u.PieceJustificatives).Returns(StubRepo(piece).Object);
            mockUow.SetupGet(u => u.Demandes).Returns(StubRepo(demande).Object);

            HistoriqueStatutDemande? capturedHist = null;
            var mockHist = new Mock<IRepository<HistoriqueStatutDemande>>();
            mockHist.Setup(r => r.AddAsync(It.IsAny<HistoriqueStatutDemande>()))
                .Callback<HistoriqueStatutDemande>(h => capturedHist = h)
                .Returns(Task.CompletedTask);
            mockUow.SetupGet(u => u.HistoriqueStatutDemandes).Returns(mockHist.Object);
            mockUow.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            var service = new DemandeService(mockUow.Object, _mapper);

            var ok = await service.ChangePieceStatusAsync(1, StatutVerification.Conforme, "RH", "ok");

            Assert.True(ok);
            Assert.NotNull(capturedHist);
            Assert.Equal(StatutDemande.Deposee, capturedHist!.Statut);
        }

        #endregion

        #region §5 eligibility rules

        [Fact]
        public async Task CreateAsync_Throws_When_TitularisationDate_Missing()
        {
            var mockUow = CreateMockUow();

            var agent = new Agent { Id = 1, SalaireMensuel = 5000 }; // no DateTitularisation
            var type = new TypeDePret { Id = 1, Plafond = 10000, FranchiseMois = 0, DureeMaxMois = 12 };

            mockUow.SetupGet(u => u.Agents).Returns(StubRepo(agent).Object);
            mockUow.SetupGet(u => u.TypeDePrets).Returns(StubRepo(type).Object);
            mockUow.SetupGet(u => u.Demandes).Returns(StubRepo<Demande>(items: new List<Demande>()).Object);

            var service = new DemandeService(mockUow.Object, _mapper);

            var dto = CreateDemandeDto(montant: 1000);

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () => await service.CreateAsync(dto));
            Assert.Contains("Date de titularisation non renseignée", ex.Message);
        }

        [Fact]
        public async Task CreateAsync_Throws_When_Seniority_Below_One_Year()
        {
            var mockUow = CreateMockUow();

            var agent = CreateEligibleAgent(dureeAnciennete: 0); // less than 1 year seniority
            var type = new TypeDePret { Id = 1, Plafond = 10000, FranchiseMois = 0, DureeMaxMois = 12 };

            mockUow.SetupGet(u => u.Agents).Returns(StubRepo(agent).Object);
            mockUow.SetupGet(u => u.TypeDePrets).Returns(StubRepo(type).Object);
            mockUow.SetupGet(u => u.Demandes).Returns(StubRepo<Demande>(items: new List<Demande>()).Object);

            var service = new DemandeService(mockUow.Object, _mapper);

            var dto = CreateDemandeDto(montant: 1000);

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () => await service.CreateAsync(dto));
            Assert.Contains("Ancienneté minimale d'un an non respectée", ex.Message);
        }

        [Fact]
        public async Task CreateAsync_Throws_When_Salary_Missing()
        {
            var mockUow = CreateMockUow();

            var agent = CreateEligibleAgent(salaire: null);
            var type = new TypeDePret { Id = 1, Plafond = 10000, FranchiseMois = 0, DureeMaxMois = 12 };

            mockUow.SetupGet(u => u.Agents).Returns(StubRepo(agent).Object);
            mockUow.SetupGet(u => u.TypeDePrets).Returns(StubRepo(type).Object);
            mockUow.SetupGet(u => u.Demandes).Returns(StubRepo<Demande>(items: new List<Demande>()).Object);
            mockUow.SetupGet(u => u.RetenuesMensuelles).Returns(StubRepo<RetenueMensuelle>(items: new List<RetenueMensuelle>()).Object);

            var service = new DemandeService(mockUow.Object, _mapper);

            var dto = CreateDemandeDto(montant: 1000);

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () => await service.CreateAsync(dto));
            Assert.Contains("Salaire mensuel non renseigné", ex.Message);
        }

        [Fact]
        public async Task CreateAsync_Throws_When_Debt_Ratio_Exceeds_40Percent()
        {
            var mockUow = CreateMockUow();

            var agent = CreateEligibleAgent(salaire: 1000m);
            var type = new TypeDePret { Id = 1, Plafond = 10000, FranchiseMois = 0, DureeMaxMois = 12 };

            var retenue = new RetenueMensuelle { Id = 1, AgentId = 1, ContratId = 1, MontantARetenir = 300m, Statut = StatutRetenue.Retenue };

            mockUow.SetupGet(u => u.Agents).Returns(StubRepo(agent).Object);
            mockUow.SetupGet(u => u.TypeDePrets).Returns(StubRepo(type).Object);
            mockUow.SetupGet(u => u.Demandes).Returns(StubRepo<Demande>(items: new List<Demande>()).Object);
            mockUow.SetupGet(u => u.RetenuesMensuelles).Returns(StubRepo<RetenueMensuelle>(items: new List<RetenueMensuelle> { retenue }).Object);

            var service = new DemandeService(mockUow.Object, _mapper);

            // mensualité estimée = 5000/12 ≈ 417 → taux = (300 + 417)/1000 = 71,7% > 40%
            var dto = CreateDemandeDto(montant: 5000);

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () => await service.CreateAsync(dto));
            Assert.Contains("Taux d'endettement", ex.Message);
        }

        [Fact]
        public async Task CreateAsync_Throws_When_Logement_Cumulative_Cap_Exceeded()
        {
            var mockUow = CreateMockUow();

            var agent = CreateEligibleAgent(salaire: 10000m);
            var type = new TypeDePret { Id = 1, Categorie = CategorieBudget.Logement, Plafond = 50000, FranchiseMois = 0, DureeMaxMois = 12 };

            var previousDemande = new Demande { Id = 10, AgentId = 1, TypeDePretId = 1, DateDepot = DateTime.UtcNow.AddMonths(-6), MontantDemande = 25000m };
            var decision = new Decision { Id = 1, DemandeId = 10, SeanceComiteId = 1, SensDecision = SensDecision.Favorable, MontantAccorde = 25000m };

            mockUow.SetupGet(u => u.Agents).Returns(StubRepo(agent).Object);
            mockUow.SetupGet(u => u.TypeDePrets).Returns(StubRepo(type, new List<TypeDePret> { type }).Object);
            mockUow.SetupGet(u => u.Demandes).Returns(StubRepo<Demande>(items: new List<Demande> { previousDemande }).Object);
            mockUow.SetupGet(u => u.RetenuesMensuelles).Returns(StubRepo<RetenueMensuelle>(items: new List<RetenueMensuelle>()).Object);
            mockUow.SetupGet(u => u.Decisions).Returns(StubRepo<Decision>(items: new List<Decision> { decision }).Object);

            var service = new DemandeService(mockUow.Object, _mapper);

            // 25 000 déjà engagés + 10 000 = 35 000 > 30 000
            var dto = CreateDemandeDto(montant: 10000);

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () => await service.CreateAsync(dto));
            Assert.Contains("Plafond cumulé logement dépassé", ex.Message);
        }

        [Fact]
        public async Task CreateAsync_Succeeds_When_All_Eligibility_Checks_Pass()
        {
            var mockUow = CreateMockUow();

            var agent = CreateEligibleAgent(salaire: 10000m);
            var type = new TypeDePret { Id = 1, Categorie = CategorieBudget.Logement, Plafond = 10000, FranchiseMois = 0, DureeMaxMois = 12 };

            mockUow.SetupGet(u => u.Agents).Returns(StubRepo(agent).Object);
            mockUow.SetupGet(u => u.TypeDePrets).Returns(StubRepo(type, new List<TypeDePret> { type }).Object);
            mockUow.SetupGet(u => u.Demandes).Returns(StubRepo<Demande>(items: new List<Demande>()).Object);
            mockUow.SetupGet(u => u.RetenuesMensuelles).Returns(StubRepo<RetenueMensuelle>(items: new List<RetenueMensuelle>()).Object);
            mockUow.SetupGet(u => u.Decisions).Returns(StubRepo<Decision>(items: new List<Decision>()).Object);
            mockUow.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            var service = new DemandeService(mockUow.Object, _mapper);

            var dto = CreateDemandeDto(montant: 5000);

            var result = await service.CreateAsync(dto);

            Assert.NotNull(result);
            Assert.Equal(StatutDemande.Deposee, result.StatutCourant);
            Assert.StartsWith("D-", result.NumeroDossier);
        }

        #endregion

        #region Transaction sérialisable (race condition sur les vérifications d'éligibilité)

        [Fact]
        public async Task CreateAsync_CommitsTransaction_OnSuccess()
        {
            var mockUow = CreateMockUow(out var mockTransaction);

            var agent = CreateEligibleAgent(salaire: 10000m);
            var type = new TypeDePret { Id = 1, Categorie = CategorieBudget.Logement, Plafond = 10000, FranchiseMois = 0, DureeMaxMois = 12 };

            mockUow.SetupGet(u => u.Agents).Returns(StubRepo(agent).Object);
            mockUow.SetupGet(u => u.TypeDePrets).Returns(StubRepo(type, new List<TypeDePret> { type }).Object);
            mockUow.SetupGet(u => u.Demandes).Returns(StubRepo<Demande>(items: new List<Demande>()).Object);
            mockUow.SetupGet(u => u.RetenuesMensuelles).Returns(StubRepo<RetenueMensuelle>(items: new List<RetenueMensuelle>()).Object);
            mockUow.SetupGet(u => u.Decisions).Returns(StubRepo<Decision>(items: new List<Decision>()).Object);
            mockUow.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            var service = new DemandeService(mockUow.Object, _mapper);

            await service.CreateAsync(CreateDemandeDto(montant: 5000));

            mockTransaction.Verify(t => t.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
            mockTransaction.Verify(t => t.RollbackAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task CreateAsync_RollsBackTransaction_OnEligibilityFailure_And_DoesNotMaskOriginalException()
        {
            var mockUow = CreateMockUow(out var mockTransaction);
            mockUow.SetupGet(u => u.Agents).Returns(StubRepo<Agent>().Object);

            var service = new DemandeService(mockUow.Object, _mapper);

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () => await service.CreateAsync(CreateDemandeDto()));

            Assert.Equal("Agent introuvable", ex.Message);
            mockTransaction.Verify(t => t.RollbackAsync(It.IsAny<CancellationToken>()), Times.Once);
            mockTransaction.Verify(t => t.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        #endregion

        #region Score de priorité (§5)

        [Fact]
        public async Task CreateAsync_Computes_Priority_Score_Celibataire_Sans_Enfants()
        {
            var mockUow = CreateMockUow();

            // 3 ans d'ancienneté, célibataire, 0 enfant → 3*4 = 12 points
            var agent = CreateEligibleAgent(salaire: 10000m, dureeAnciennete: 3);
            agent.SituationFamiliale = SituationFamiliale.Celibataire;
            agent.NombreEnfantsACharge = 0;

            var type = new TypeDePret { Id = 1, Plafond = 10000, FranchiseMois = 0, DureeMaxMois = 12 };

            mockUow.SetupGet(u => u.Agents).Returns(StubRepo(agent).Object);
            mockUow.SetupGet(u => u.TypeDePrets).Returns(StubRepo(type).Object);
            mockUow.SetupGet(u => u.Demandes).Returns(StubRepo<Demande>(items: new List<Demande>()).Object);
            mockUow.SetupGet(u => u.RetenuesMensuelles).Returns(StubRepo<RetenueMensuelle>(items: new List<RetenueMensuelle>()).Object);
            mockUow.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            var service = new DemandeService(mockUow.Object, _mapper);

            var dto = CreateDemandeDto(montant: 1000);

            var result = await service.CreateAsync(dto);

            Assert.Equal(12m, result.ScorePriorite);
        }

        [Fact]
        public async Task CreateAsync_Computes_Priority_Score_Marie_Avec_Enfants()
        {
            var mockUow = CreateMockUow();

            // 2 ans d'ancienneté, marié, 2 enfants → 8 + 4 + 4 = 16 points
            var agent = CreateEligibleAgent(salaire: 10000m, dureeAnciennete: 2);
            agent.SituationFamiliale = SituationFamiliale.Marie;
            agent.NombreEnfantsACharge = 2;

            var type = new TypeDePret { Id = 1, Plafond = 10000, FranchiseMois = 0, DureeMaxMois = 12 };

            mockUow.SetupGet(u => u.Agents).Returns(StubRepo(agent).Object);
            mockUow.SetupGet(u => u.TypeDePrets).Returns(StubRepo(type).Object);
            mockUow.SetupGet(u => u.Demandes).Returns(StubRepo<Demande>(items: new List<Demande>()).Object);
            mockUow.SetupGet(u => u.RetenuesMensuelles).Returns(StubRepo<RetenueMensuelle>(items: new List<RetenueMensuelle>()).Object);
            mockUow.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            var service = new DemandeService(mockUow.Object, _mapper);

            var dto = CreateDemandeDto(montant: 1000);

            var result = await service.CreateAsync(dto);

            Assert.Equal(16m, result.ScorePriorite);
        }

        [Fact]
        public async Task CreateAsync_Computes_Priority_Score_Divorce_Avec_Garde_Enfants()
        {
            var mockUow = CreateMockUow();

            // 5 ans d'ancienneté, divorcé avec 1 enfant → 20 + 4 + 2 = 26 points
            var agent = CreateEligibleAgent(salaire: 10000m, dureeAnciennete: 5);
            agent.SituationFamiliale = SituationFamiliale.Divorce;
            agent.NombreEnfantsACharge = 1;

            var type = new TypeDePret { Id = 1, Plafond = 10000, FranchiseMois = 0, DureeMaxMois = 12 };

            mockUow.SetupGet(u => u.Agents).Returns(StubRepo(agent).Object);
            mockUow.SetupGet(u => u.TypeDePrets).Returns(StubRepo(type).Object);
            mockUow.SetupGet(u => u.Demandes).Returns(StubRepo<Demande>(items: new List<Demande>()).Object);
            mockUow.SetupGet(u => u.RetenuesMensuelles).Returns(StubRepo<RetenueMensuelle>(items: new List<RetenueMensuelle>()).Object);
            mockUow.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            var service = new DemandeService(mockUow.Object, _mapper);

            var dto = CreateDemandeDto(montant: 1000);

            var result = await service.CreateAsync(dto);

            Assert.Equal(26m, result.ScorePriorite);
        }

        [Fact]
        public async Task CreateAsync_Computes_Priority_Score_Veuf_Sans_Enfants_Pas_De_Bonus_Garde()
        {
            var mockUow = CreateMockUow();

            // 1 an d'ancienneté, veuf sans enfant → 4 points (pas de bonus garde d'enfants)
            var agent = CreateEligibleAgent(salaire: 10000m, dureeAnciennete: 1);
            agent.SituationFamiliale = SituationFamiliale.Veuf;
            agent.NombreEnfantsACharge = 0;

            var type = new TypeDePret { Id = 1, Plafond = 10000, FranchiseMois = 0, DureeMaxMois = 12 };

            mockUow.SetupGet(u => u.Agents).Returns(StubRepo(agent).Object);
            mockUow.SetupGet(u => u.TypeDePrets).Returns(StubRepo(type).Object);
            mockUow.SetupGet(u => u.Demandes).Returns(StubRepo<Demande>(items: new List<Demande>()).Object);
            mockUow.SetupGet(u => u.RetenuesMensuelles).Returns(StubRepo<RetenueMensuelle>(items: new List<RetenueMensuelle>()).Object);
            mockUow.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            var service = new DemandeService(mockUow.Object, _mapper);

            var dto = CreateDemandeDto(montant: 1000);

            var result = await service.CreateAsync(dto);

            Assert.Equal(4m, result.ScorePriorite);
        }

        #endregion
    }
}