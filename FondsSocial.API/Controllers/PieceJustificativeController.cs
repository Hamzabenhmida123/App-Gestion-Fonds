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
    public class PieceJustificativeController : ControllerBase
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public PieceJustificativeController(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _uow.PieceJustificatives.GetAllAsync();
            return Ok(_mapper.Map<IEnumerable<PieceJustificativeDto>>(list));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var e = await _uow.PieceJustificatives.GetByIdAsync(id);
            if (e == null) return NotFound();
            return Ok(_mapper.Map<PieceJustificativeDto>(e));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreatePieceJustificativeDto dto)
        {
            var entity = _mapper.Map<PieceJustificative>(dto);
            await _uow.PieceJustificatives.AddAsync(entity);
            await _uow.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = entity.Id }, _mapper.Map<PieceJustificativeDto>(entity));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdatePieceJustificativeDto dto)
        {
            var existing = await _uow.PieceJustificatives.GetByIdAsync(id);
            if (existing == null) return NotFound();
            _mapper.Map(dto, existing);
            _uow.PieceJustificatives.Update(existing);
            await _uow.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var existing = await _uow.PieceJustificatives.GetByIdAsync(id);
            if (existing == null) return NotFound();
            _uow.PieceJustificatives.Delete(existing);
            await _uow.SaveChangesAsync();
            return NoContent();
        }
    }
}
