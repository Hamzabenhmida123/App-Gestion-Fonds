using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using FondsSocial.Domain.Entities;
using FondsSocial.Infrastructure.UnitOfWork;

namespace FondsSocial.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SeanceComiteController : ControllerBase
    {
        private readonly IUnitOfWork _uow;

        public SeanceComiteController(IUnitOfWork uow)
        {
            _uow = uow;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _uow.SeanceComites.GetAllAsync();
            return Ok(list);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var e = await _uow.SeanceComites.GetByIdAsync(id);
            if (e == null) return NotFound();
            return Ok(e);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] SeanceComite dto)
        {
            await _uow.SeanceComites.AddAsync(dto);
            await _uow.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = dto.Id }, dto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] SeanceComite dto)
        {
            var existing = await _uow.SeanceComites.GetByIdAsync(id);
            if (existing == null) return NotFound();
            dto.Id = id;
            _uow.SeanceComites.Update(dto);
            await _uow.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var existing = await _uow.SeanceComites.GetByIdAsync(id);
            if (existing == null) return NotFound();
            _uow.SeanceComites.Delete(existing);
            await _uow.SaveChangesAsync();
            return NoContent();
        }
    }
}
