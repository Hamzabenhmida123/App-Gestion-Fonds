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
    public class EcheanceController : ControllerBase
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public EcheanceController(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _uow.Echeances.GetAllAsync();
            return Ok(_mapper.Map<IEnumerable<EcheanceDto>>(list));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var e = await _uow.Echeances.GetByIdAsync(id);
            if (e == null) return NotFound();
            return Ok(_mapper.Map<EcheanceDto>(e));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateEcheanceDto dto)
        {
            var entity = _mapper.Map<Echeance>(dto);
            await _uow.Echeances.AddAsync(entity);
            await _uow.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = entity.Id }, _mapper.Map<EcheanceDto>(entity));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateEcheanceDto dto)
        {
            var existing = await _uow.Echeances.GetByIdAsync(id);
            if (existing == null) return NotFound();
            _mapper.Map(dto, existing);
            _uow.Echeances.Update(existing);
            await _uow.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var existing = await _uow.Echeances.GetByIdAsync(id);
            if (existing == null) return NotFound();
            _uow.Echeances.Delete(existing);
            await _uow.SaveChangesAsync();
            return NoContent();
        }
    }
}
