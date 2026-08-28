using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using FondsSocial.Application.DTOs;
using FondsSocial.Domain.Entities;
using FondsSocial.Infrastructure.UnitOfWork;

namespace FondsSocial.API.Controllers
{
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
        public async Task<IActionResult> GetAll()
        {
            var list = await _uow.HistoriqueStatutDemandes.GetAllAsync();
            return Ok(_mapper.Map<IEnumerable<HistoriqueStatutDemandeDto>>(list));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var e = await _uow.HistoriqueStatutDemandes.GetByIdAsync(id);
            if (e == null) return NotFound();
            return Ok(_mapper.Map<HistoriqueStatutDemandeDto>(e));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateHistoriqueStatutDemandeDto dto)
        {
            var entity = _mapper.Map<HistoriqueStatutDemande>(dto);
            await _uow.HistoriqueStatutDemandes.AddAsync(entity);
            await _uow.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = entity.Id }, _mapper.Map<HistoriqueStatutDemandeDto>(entity));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateHistoriqueStatutDemandeDto dto)
        {
            var existing = await _uow.HistoriqueStatutDemandes.GetByIdAsync(id);
            if (existing == null) return NotFound();
            _mapper.Map(dto, existing);
            _uow.HistoriqueStatutDemandes.Update(existing);
            await _uow.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var existing = await _uow.HistoriqueStatutDemandes.GetByIdAsync(id);
            if (existing == null) return NotFound();
            _uow.HistoriqueStatutDemandes.Delete(existing);
            await _uow.SaveChangesAsync();
            return NoContent();
        }
    }
}
