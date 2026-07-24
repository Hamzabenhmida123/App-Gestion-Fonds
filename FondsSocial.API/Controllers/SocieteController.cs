using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using FondsSocial.Application.DTOs;
using FondsSocial.Domain.Entities;
using FondsSocial.Infrastructure.UnitOfWork;

namespace FondsSocial.API.Controllers
{
    /// <summary>
    /// API CRUD pour les sociétés
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class SocieteController : ControllerBase
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public SocieteController(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        /// <summary>Récupère toutes les sociétés</summary>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _uow.Societes.GetAllAsync();
            return Ok(_mapper.Map<IEnumerable<SocieteDto>>(list));
        }

        /// <summary>Récupère une société par id</summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var e = await _uow.Societes.GetByIdAsync(id);
            if (e == null) return NotFound();
            return Ok(_mapper.Map<SocieteDto>(e));
        }

        /// <summary>Crée une nouvelle société</summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateSocieteDto dto)
        {
            var entity = _mapper.Map<Societe>(dto);
            await _uow.Societes.AddAsync(entity);
            await _uow.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = entity.Id }, _mapper.Map<SocieteDto>(entity));
        }

        /// <summary>Met à jour une société</summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateSocieteDto dto)
        {
            var existing = await _uow.Societes.GetByIdAsync(id);
            if (existing == null) return NotFound();
            _mapper.Map(dto, existing);
            _uow.Societes.Update(existing);
            await _uow.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>Supprime une société</summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var existing = await _uow.Societes.GetByIdAsync(id);
            if (existing == null) return NotFound();
            _uow.Societes.Delete(existing);
            await _uow.SaveChangesAsync();
            return NoContent();
        }
    }
}
