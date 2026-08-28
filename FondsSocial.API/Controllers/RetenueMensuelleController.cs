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
    public class RetenueMensuelleController : ControllerBase
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public RetenueMensuelleController(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _uow.RetenuesMensuelles.GetAllAsync();
            return Ok(_mapper.Map<IEnumerable<RetenueMensuelleDto>>(list));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var e = await _uow.RetenuesMensuelles.GetByIdAsync(id);
            if (e == null) return NotFound();
            return Ok(_mapper.Map<RetenueMensuelleDto>(e));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateRetenueMensuelleDto dto)
        {
            var entity = _mapper.Map<RetenueMensuelle>(dto);
            await _uow.RetenuesMensuelles.AddAsync(entity);
            await _uow.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = entity.Id }, _mapper.Map<RetenueMensuelleDto>(entity));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateRetenueMensuelleDto dto)
        {
            var existing = await _uow.RetenuesMensuelles.GetByIdAsync(id);
            if (existing == null) return NotFound();
            _mapper.Map(dto, existing);
            _uow.RetenuesMensuelles.Update(existing);
            await _uow.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var existing = await _uow.RetenuesMensuelles.GetByIdAsync(id);
            if (existing == null) return NotFound();
            _uow.RetenuesMensuelles.Delete(existing);
            await _uow.SaveChangesAsync();
            return NoContent();
        }
    }
}
