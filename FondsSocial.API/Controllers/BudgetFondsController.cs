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
    public class BudgetFondsController : ControllerBase
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public BudgetFondsController(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _uow.BudgetsFonds.GetAllAsync();
            return Ok(_mapper.Map<IEnumerable<BudgetFondsDto>>(list));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var e = await _uow.BudgetsFonds.GetByIdAsync(id);
            if (e == null) return NotFound();
            return Ok(_mapper.Map<BudgetFondsDto>(e));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateBudgetFondsDto dto)
        {
            var entity = _mapper.Map<BudgetFonds>(dto);
            await _uow.BudgetsFonds.AddAsync(entity);
            await _uow.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = entity.Id }, _mapper.Map<BudgetFondsDto>(entity));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateBudgetFondsDto dto)
        {
            var existing = await _uow.BudgetsFonds.GetByIdAsync(id);
            if (existing == null) return NotFound();
            _mapper.Map(dto, existing);
            _uow.BudgetsFonds.Update(existing);
            await _uow.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var existing = await _uow.BudgetsFonds.GetByIdAsync(id);
            if (existing == null) return NotFound();
            _uow.BudgetsFonds.Delete(existing);
            await _uow.SaveChangesAsync();
            return NoContent();
        }
    }
}
