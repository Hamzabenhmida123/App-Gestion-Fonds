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
    public class GarantieController : ControllerBase
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public GarantieController(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _uow.Garanties.GetAllAsync();
            return Ok(_mapper.Map<IEnumerable<GarantieDto>>(list));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var e = await _uow.Garanties.GetByIdAsync(id);
            if (e == null) return NotFound();
            return Ok(_mapper.Map<GarantieDto>(e));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateGarantieDto dto)
        {
            var entity = _mapper.Map<Garantie>(dto);
            await _uow.Garanties.AddAsync(entity);
            await _uow.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = entity.Id }, _mapper.Map<GarantieDto>(entity));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateGarantieDto dto)
        {
            var existing = await _uow.Garanties.GetByIdAsync(id);
            if (existing == null) return NotFound();
            _mapper.Map(dto, existing);
            _uow.Garanties.Update(existing);
            await _uow.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var existing = await _uow.Garanties.GetByIdAsync(id);
            if (existing == null) return NotFound();
            _uow.Garanties.Delete(existing);
            await _uow.SaveChangesAsync();
            return NoContent();
        }
    }
}
