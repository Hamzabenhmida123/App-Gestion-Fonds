using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
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

        [Fact]
        public async Task CreateAsync_Throws_When_Montant_Exceeds_Plafond()
        {
            var mockUow = new Mock<IUnitOfWork>();

            var agent = new Agent { Id = 1 };
            var type = new TypeDePret { Id = 1, Plafond = 1000, FranchiseMois = 0 };

            var mockAgents = new Mock<IRepository<Agent>>();
            mockAgents.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(agent);
            mockUow.SetupGet(u => u.Agents).Returns(mockAgents.Object);

            var mockTypes = new Mock<IRepository<TypeDePret>>();
            mockTypes.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(type);
            mockUow.SetupGet(u => u.TypeDePrets).Returns(mockTypes.Object);

            var mockDemandes = new Mock<IRepository<Demande>>();
            mockDemandes.Setup(r => r.FindAsync(It.IsAny<Expression<Func<Demande,bool>>>())).ReturnsAsync(new List<Demande>());
            mockUow.SetupGet(u => u.Demandes).Returns(mockDemandes.Object);

            var service = new DemandeService(mockUow.Object, _mapper);

            var dto = new CreateDemandeDto { AgentId = 1, TypeDePretId = 1, MontantDemande = 2000, DateDepot = DateTime.UtcNow };

            await Assert.ThrowsAsync<InvalidOperationException>(async () => await service.CreateAsync(dto));
        }

        [Fact]
        public async Task CreateAsync_Throws_When_Franchise_Not_Respected()
        {
            var mockUow = new Mock<IUnitOfWork>();

            var agent = new Agent { Id = 1 };
            var type = new TypeDePret { Id = 1, Plafond = 10000, FranchiseMois = 12 };

            var previous = new Demande { Id = 5, AgentId = 1, TypeDePretId = 1, DateDepot = DateTime.UtcNow.AddMonths(-1) };

            var mockAgents = new Mock<IRepository<Agent>>();
            mockAgents.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(agent);
            mockUow.SetupGet(u => u.Agents).Returns(mockAgents.Object);

            var mockTypes = new Mock<IRepository<TypeDePret>>();
            mockTypes.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(type);
            mockUow.SetupGet(u => u.TypeDePrets).Returns(mockTypes.Object);

            var mockDemandes = new Mock<IRepository<Demande>>();
            mockDemandes.Setup(r => r.FindAsync(It.IsAny<Expression<Func<Demande,bool>>>())).ReturnsAsync(new List<Demande> { previous });
            mockUow.SetupGet(u => u.Demandes).Returns(mockDemandes.Object);

            var service = new DemandeService(mockUow.Object, _mapper);

            var dto = new CreateDemandeDto { AgentId = 1, TypeDePretId = 1, MontantDemande = 1000, DateDepot = DateTime.UtcNow };

            await Assert.ThrowsAsync<InvalidOperationException>(async () => await service.CreateAsync(dto));
        }

        [Fact]
        public async Task TransitionStatutAsync_Allows_Valid_Transition()
        {
            var mockUow = new Mock<IUnitOfWork>();

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
    }
}
