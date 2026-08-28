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
    public class MembreComiteController : ControllerBase
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public MembreComiteController(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _uow.MembreComites.GetAllAsync();
            return Ok(_mapper.Map<IEnumerable<MembreComiteDto>>(list));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var e = await _uow.MembreComites.GetByIdAsync(id);
            if (e == null) return NotFound();
            return Ok(_mapper.Map<MembreComiteDto>(e));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateMembreComiteDto dto)
        {
            var entity = _mapper.Map<MembreComite>(dto);
            await _uow.MembreComites.AddAsync(entity);
            await _uow.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = entity.Id }, _mapper.Map<MembreComiteDto>(entity));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateMembreComiteDto dto)
        {
            var existing = await _uow.MembreComites.GetByIdAsync(id);
            if (existing == null) return NotFound();
            _mapper.Map(dto, existing);
            _uow.MembreComites.Update(existing);
            await _uow.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var existing = await _uow.MembreComites.GetByIdAsync(id);
            if (existing == null) return NotFound();
            _uow.MembreComites.Delete(existing);
            await _uow.SaveChangesAsync();
            return NoContent();
        }
    }
}
