using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using FondsSocial.Application.DTOs;
using FondsSocial.Infrastructure.UnitOfWork;

namespace FondsSocial.API.Controllers
{
    /// <summary>
    /// Lecture seule: l'historique des statuts ne doit être alimenté que par DemandeService
    /// (transitions, clôture de dépôt, etc.), jamais par écriture directe d'un client, qui
    /// permettrait de forger un historique déconnecté de l'état réel de la demande.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class HistoriqueStatutDemandeController : ControllerBase
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public HistoriqueStatutDemandeController(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 20;
            if (pageSize > 100) pageSize = 100;

            var (items, totalCount) = await _uow.HistoriqueStatutDemandes.GetPagedAsync(page, pageSize);
            return Ok(new PagedResult<HistoriqueStatutDemandeDto>
            {
                Items = _mapper.Map<IEnumerable<HistoriqueStatutDemandeDto>>(items),
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var e = await _uow.HistoriqueStatutDemandes.GetByIdAsync(id);
            if (e == null) return NotFound();
            return Ok(_mapper.Map<HistoriqueStatutDemandeDto>(e));
        }
    }
}
