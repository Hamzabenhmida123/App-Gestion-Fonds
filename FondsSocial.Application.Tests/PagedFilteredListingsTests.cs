#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Moq;
using Xunit;
using AutoMapper;
using FondsSocial.Application.Services;
using FondsSocial.Application.DTOs;
using FondsSocial.Application.Mapping;
using FondsSocial.Domain.Entities;
using FondsSocial.Domain.Enums;
using FondsSocial.Infrastructure.UnitOfWork;
using FondsSocial.Infrastructure.Repositories;

namespace FondsSocial.Application.Tests
{
    /// <summary>
    /// Vérifie que les listages de Decision, Echeance, Garantie, RetenueMensuelle et
    /// ParticipationSeance filtrent et paginent réellement au niveau du repository (au lieu
    /// de se contenter d'un mock trivial qui renverrait toujours tout), à l'image du test déjà
    /// existant pour DemandeService.GetAllFilteredAsync.
    /// </summary>
    public class PagedFilteredListingsTests
    {
        private readonly IMapper _mapper;

        public PagedFilteredListingsTests()
        {
            var cfg = new MapperConfiguration(cfg => cfg.AddProfile(new MappingProfile()));
            _mapper = cfg.CreateMapper();
        }

        /// <summary>
        /// Simule ce que fait réellement Repository&lt;T&gt;.GetPagedAsync contre la base
        /// (Where + Count + OrderBy + Skip/Take), pour que ces tests détectent une expression
        /// de filtre incorrecte au lieu de toujours renvoyer la liste complète.
        /// </summary>
        private static Mock<IRepository<T>> PagedRepo<T>(List<T> data) where T : class
        {
            var repo = new Mock<IRepository<T>>();
            repo.Setup(r => r.GetPagedAsync(
                    It.IsAny<int>(), It.IsAny<int>(),
                    It.IsAny<Expression<Func<T, bool>>>(),
                    It.IsAny<Func<IQueryable<T>, IOrderedQueryable<T>>>()))
                .ReturnsAsync((int page, int pageSize, Expression<Func<T, bool>>? filter, Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy) =>
                {
                    IQueryable<T> query = data.AsQueryable();
                    if (filter != null) query = query.Where(filter);
                    var total = query.Count();
                    if (orderBy != null) query = orderBy(query);
                    var items = query.Skip((page - 1) * pageSize).Take(pageSize).ToList();
                    return ((IEnumerable<T>)items, total);
                });
            return repo;
        }

        #region DecisionService

        [Fact]
        public async Task DecisionService_GetAllAsync_Filters_By_SeanceComiteId_And_Paginates()
        {
            var decisions = new List<Decision>
            {
                new Decision { Id = 1, DemandeId = 1, SeanceComiteId = 1 },
                new Decision { Id = 2, DemandeId = 2, SeanceComiteId = 1 },
                new Decision { Id = 3, DemandeId = 3, SeanceComiteId = 1 },
                new Decision { Id = 4, DemandeId = 4, SeanceComiteId = 2 },
            };

            var mockUow = new Mock<IUnitOfWork>();
            mockUow.SetupGet(u => u.Decisions).Returns(PagedRepo(decisions).Object);

            var service = new DecisionService(mockUow.Object, _mapper, Mock.Of<IDemandeService>());

            var result = await service.GetAllAsync(seanceComiteId: 1, page: 1, pageSize: 2);

            Assert.Equal(3, result.TotalCount); // séance 1 a 3 décisions au total
            Assert.Equal(2, result.Items.Count()); // pageSize = 2
            Assert.All(result.Items, d => Assert.Equal(1, d.SeanceComiteId));
        }

        [Fact]
        public async Task DecisionService_GetAllAsync_Without_Filter_Returns_Every_Seance()
        {
            var decisions = new List<Decision>
            {
                new Decision { Id = 1, DemandeId = 1, SeanceComiteId = 1 },
                new Decision { Id = 2, DemandeId = 2, SeanceComiteId = 2 },
            };

            var mockUow = new Mock<IUnitOfWork>();
            mockUow.SetupGet(u => u.Decisions).Returns(PagedRepo(decisions).Object);

            var service = new DecisionService(mockUow.Object, _mapper, Mock.Of<IDemandeService>());

            var result = await service.GetAllAsync(seanceComiteId: null, page: 1, pageSize: 100);

            Assert.Equal(2, result.TotalCount);
        }

        #endregion

        #region EcheanceService

        [Fact]
        public async Task EcheanceService_GetAllAsync_Filters_By_ContratId_And_Orders_By_NumeroEcheance()
        {
            var echeances = new List<Echeance>
            {
                new Echeance { Id = 1, ContratId = 1, NumeroEcheance = 3 },
                new Echeance { Id = 2, ContratId = 1, NumeroEcheance = 1 },
                new Echeance { Id = 3, ContratId = 1, NumeroEcheance = 2 },
                new Echeance { Id = 4, ContratId = 2, NumeroEcheance = 1 },
            };

            var mockUow = new Mock<IUnitOfWork>();
            mockUow.SetupGet(u => u.Echeances).Returns(PagedRepo(echeances).Object);

            var service = new EcheanceService(mockUow.Object, _mapper);

            var result = await service.GetAllAsync(contratId: 1, page: 1, pageSize: 10);

            Assert.Equal(3, result.TotalCount); // contrat 2 exclu
            Assert.Equal(new[] { 1, 2, 3 }, result.Items.Select(e => e.NumeroEcheance)); // triées par numéro croissant
        }

        [Fact]
        public async Task EcheanceService_GetAllAsync_Paginates_Second_Page()
        {
            var echeances = Enumerable.Range(1, 5)
                .Select(n => new Echeance { Id = n, ContratId = 1, NumeroEcheance = n })
                .ToList();

            var mockUow = new Mock<IUnitOfWork>();
            mockUow.SetupGet(u => u.Echeances).Returns(PagedRepo(echeances).Object);

            var service = new EcheanceService(mockUow.Object, _mapper);

            var result = await service.GetAllAsync(contratId: 1, page: 2, pageSize: 2);

            Assert.Equal(5, result.TotalCount);
            Assert.Equal(new[] { 3, 4 }, result.Items.Select(e => e.NumeroEcheance));
        }

        #endregion

        #region GarantieService

        [Fact]
        public async Task GarantieService_GetAllAsync_Filters_By_ContratId()
        {
            var garanties = new List<Garantie>
            {
                new Garantie { Id = 1, ContratId = 1 },
                new Garantie { Id = 2, ContratId = 2 },
            };

            var mockUow = new Mock<IUnitOfWork>();
            mockUow.SetupGet(u => u.Garanties).Returns(PagedRepo(garanties).Object);

            var service = new GarantieService(mockUow.Object, _mapper);

            var result = await service.GetAllAsync(contratId: 2, page: 1, pageSize: 10);

            Assert.Equal(1, result.TotalCount);
            Assert.Equal(2, result.Items.Single().ContratId);
        }

        [Fact]
        public async Task GarantieService_GetAllAsync_Clamps_Invalid_Page_And_PageSize()
        {
            var mockUow = new Mock<IUnitOfWork>();
            mockUow.SetupGet(u => u.Garanties).Returns(PagedRepo(new List<Garantie>()).Object);

            var service = new GarantieService(mockUow.Object, _mapper);

            var result = await service.GetAllAsync(contratId: null, page: 0, pageSize: 1000);

            Assert.Equal(1, result.Page); // page < 1 ramenée à 1
            Assert.Equal(100, result.PageSize); // pageSize plafonnée à 100
        }

        #endregion

        #region RetenueMensuelleService

        [Fact]
        public async Task RetenueMensuelleService_GetAllAsync_Filters_By_ContratId_And_Paginates()
        {
            var retenues = new List<RetenueMensuelle>
            {
                new RetenueMensuelle { Id = 1, ContratId = 1, AgentId = 1 },
                new RetenueMensuelle { Id = 2, ContratId = 1, AgentId = 1 },
                new RetenueMensuelle { Id = 3, ContratId = 2, AgentId = 2 },
            };

            var mockUow = new Mock<IUnitOfWork>();
            mockUow.SetupGet(u => u.RetenuesMensuelles).Returns(PagedRepo(retenues).Object);

            var service = new RetenueMensuelleService(mockUow.Object, _mapper);

            var result = await service.GetAllAsync(contratId: 1, page: 1, pageSize: 1);

            Assert.Equal(2, result.TotalCount); // contrat 1 a 2 retenues au total
            Assert.Single(result.Items); // pageSize = 1
            Assert.All(result.Items, r => Assert.Equal(1, r.ContratId));
        }

        #endregion

        #region ParticipationSeanceService

        [Fact]
        public async Task ParticipationSeanceService_GetAllAsync_Filters_By_SeanceComiteId()
        {
            var participations = new List<ParticipationSeance>
            {
                new ParticipationSeance { Id = 1, SeanceComiteId = 1, MembreComiteId = 1 },
                new ParticipationSeance { Id = 2, SeanceComiteId = 1, MembreComiteId = 2 },
                new ParticipationSeance { Id = 3, SeanceComiteId = 2, MembreComiteId = 1 },
            };

            var mockUow = new Mock<IUnitOfWork>();
            mockUow.SetupGet(u => u.ParticipationSeances).Returns(PagedRepo(participations).Object);

            var service = new ParticipationSeanceService(mockUow.Object, _mapper);

            var result = await service.GetAllAsync(seanceComiteId: 1, page: 1, pageSize: 10);

            Assert.Equal(2, result.TotalCount);
            Assert.All(result.Items, p => Assert.Equal(1, p.SeanceComiteId));
        }

        [Fact]
        public async Task ParticipationSeanceService_GetAllAsync_Without_Filter_Returns_Every_Seance()
        {
            var participations = new List<ParticipationSeance>
            {
                new ParticipationSeance { Id = 1, SeanceComiteId = 1, MembreComiteId = 1 },
                new ParticipationSeance { Id = 2, SeanceComiteId = 2, MembreComiteId = 1 },
            };

            var mockUow = new Mock<IUnitOfWork>();
            mockUow.SetupGet(u => u.ParticipationSeances).Returns(PagedRepo(participations).Object);

            var service = new ParticipationSeanceService(mockUow.Object, _mapper);

            var result = await service.GetAllAsync(seanceComiteId: null, page: 1, pageSize: 100);

            Assert.Equal(2, result.TotalCount);
        }

        #endregion
    }
}
