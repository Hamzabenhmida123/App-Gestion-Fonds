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
    public class DecisionController : ControllerBase
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public DecisionController(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _uow.Decisions.GetAllAsync();
            return Ok(_mapper.Map<IEnumerable<DecisionDto>>(list));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var e = await _uow.Decisions.GetByIdAsync(id);
            if (e == null) return NotFound();
            return Ok(_mapper.Map<DecisionDto>(e));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateDecisionDto dto)
        {
            var entity = _mapper.Map<Decision>(dto);
            await _uow.Decisions.AddAsync(entity);
            await _uow.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = entity.Id }, _mapper.Map<DecisionDto>(entity));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateDecisionDto dto)
        {
            var existing = await _uow.Decisions.GetByIdAsync(id);
            if (existing == null) return NotFound();
            _mapper.Map(dto, existing);
            _uow.Decisions.Update(existing);
            await _uow.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var existing = await _uow.Decisions.GetByIdAsync(id);
            if (existing == null) return NotFound();
            _uow.Decisions.Delete(existing);
            await _uow.SaveChangesAsync();
            return NoContent();
        }
    }
}
