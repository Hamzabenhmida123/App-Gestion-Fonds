using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using FondsSocial.Domain.Entities;
using FondsSocial.Infrastructure.UnitOfWork;

namespace FondsSocial.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DecisionController : ControllerBase
    {
        private readonly IUnitOfWork _uow;

        public DecisionController(IUnitOfWork uow)
        {
            _uow = uow;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _uow.Decisions.GetAllAsync();
            return Ok(list);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var e = await _uow.Decisions.GetByIdAsync(id);
            if (e == null) return NotFound();
            return Ok(e);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Decision dto)
        {
            await _uow.Decisions.AddAsync(dto);
            await _uow.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = dto.Id }, dto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Decision dto)
        {
            var existing = await _uow.Decisions.GetByIdAsync(id);
            if (existing == null) return NotFound();
            dto.Id = id;
            _uow.Decisions.Update(dto);
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
