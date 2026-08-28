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
    public class ContratController : ControllerBase
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public ContratController(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _uow.Contrats.GetAllAsync();
            return Ok(_mapper.Map<IEnumerable<ContratDto>>(list));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var e = await _uow.Contrats.GetByIdAsync(id);
            if (e == null) return NotFound();
            return Ok(_mapper.Map<ContratDto>(e));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateContratDto dto)
        {
            var entity = _mapper.Map<Contrat>(dto);
            await _uow.Contrats.AddAsync(entity);
            await _uow.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = entity.Id }, _mapper.Map<ContratDto>(entity));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateContratDto dto)
        {
            var existing = await _uow.Contrats.GetByIdAsync(id);
            if (existing == null) return NotFound();
            _mapper.Map(dto, existing);
            _uow.Contrats.Update(existing);
            await _uow.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var existing = await _uow.Contrats.GetByIdAsync(id);
            if (existing == null) return NotFound();
            _uow.Contrats.Delete(existing);
            await _uow.SaveChangesAsync();
            return NoContent();
        }
    }
}
