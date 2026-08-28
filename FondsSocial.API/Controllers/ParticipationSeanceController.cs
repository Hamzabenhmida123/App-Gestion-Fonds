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
    public class ParticipationSeanceController : ControllerBase
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public ParticipationSeanceController(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _uow.ParticipationSeances.GetAllAsync();
            return Ok(_mapper.Map<IEnumerable<ParticipationSeanceDto>>(list));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var e = await _uow.ParticipationSeances.GetByIdAsync(id);
            if (e == null) return NotFound();
            return Ok(_mapper.Map<ParticipationSeanceDto>(e));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateParticipationSeanceDto dto)
        {
            var entity = _mapper.Map<ParticipationSeance>(dto);
            await _uow.ParticipationSeances.AddAsync(entity);
            await _uow.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = entity.Id }, _mapper.Map<ParticipationSeanceDto>(entity));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateParticipationSeanceDto dto)
        {
            var existing = await _uow.ParticipationSeances.GetByIdAsync(id);
            if (existing == null) return NotFound();
            _mapper.Map(dto, existing);
            _uow.ParticipationSeances.Update(existing);
            await _uow.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var existing = await _uow.ParticipationSeances.GetByIdAsync(id);
            if (existing == null) return NotFound();
            _uow.ParticipationSeances.Delete(existing);
            await _uow.SaveChangesAsync();
            return NoContent();
        }
    }
}
