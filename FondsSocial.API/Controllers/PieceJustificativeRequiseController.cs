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
    /// CRUD pour les pièces justificatives requises
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class PieceJustificativeRequiseController : ControllerBase
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public PieceJustificativeRequiseController(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _uow.PieceJustificativeRequises.GetAllAsync();
            return Ok(_mapper.Map<IEnumerable<PieceJustificativeRequiseDto>>(list));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var e = await _uow.PieceJustificativeRequises.GetByIdAsync(id);
            if (e == null) return NotFound();
            return Ok(_mapper.Map<PieceJustificativeRequiseDto>(e));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreatePieceJustificativeRequiseDto dto)
        {
            var entity = _mapper.Map<PieceJustificativeRequise>(dto);
            await _uow.PieceJustificativeRequises.AddAsync(entity);
            await _uow.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = entity.Id }, _mapper.Map<PieceJustificativeRequiseDto>(entity));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdatePieceJustificativeRequiseDto dto)
        {
            var existing = await _uow.PieceJustificativeRequises.GetByIdAsync(id);
            if (existing == null) return NotFound();
            _mapper.Map(dto, existing);
            _uow.PieceJustificativeRequises.Update(existing);
            await _uow.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var existing = await _uow.PieceJustificativeRequises.GetByIdAsync(id);
            if (existing == null) return NotFound();
            _uow.PieceJustificativeRequises.Delete(existing);
            await _uow.SaveChangesAsync();
            return NoContent();
        }
    }
}
