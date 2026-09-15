using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using FondsSocial.Application.DTOs;
using FondsSocial.Domain.Entities;
using FondsSocial.Infrastructure.UnitOfWork;

namespace FondsSocial.API.Controllers
{
    /// <summary>
    /// CRUD pour les pièces justificatives requises
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class PieceJustificativeRequiseController : ControllerBase
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public PieceJustificativeRequiseController(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int? typeDePretId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 20;
            if (pageSize > 100) pageSize = 100;

            System.Linq.Expressions.Expression<Func<PieceJustificativeRequise, bool>>? filter = typeDePretId.HasValue
                ? p => p.TypeDePretId == typeDePretId.Value
                : null;

            var (items, totalCount) = await _uow.PieceJustificativeRequises.GetPagedAsync(page, pageSize, filter);
            return Ok(new PagedResult<PieceJustificativeRequiseDto>
            {
                Items = _mapper.Map<IEnumerable<PieceJustificativeRequiseDto>>(items),
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var e = await _uow.PieceJustificativeRequises.GetByIdAsync(id);
            if (e == null) return NotFound();
            return Ok(_mapper.Map<PieceJustificativeRequiseDto>(e));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreatePieceJustificativeRequiseDto dto)
        {
            var entity = _mapper.Map<PieceJustificativeRequise>(dto);
            await _uow.PieceJustificativeRequises.AddAsync(entity);
            await _uow.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = entity.Id }, _mapper.Map<PieceJustificativeRequiseDto>(entity));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdatePieceJustificativeRequiseDto dto)
        {
            var existing = await _uow.PieceJustificativeRequises.GetByIdAsync(id);
            if (existing == null) return NotFound();
            _mapper.Map(dto, existing);
            _uow.PieceJustificativeRequises.Update(existing);
            await _uow.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var existing = await _uow.PieceJustificativeRequises.GetByIdAsync(id);
            if (existing == null) return NotFound();
            _uow.PieceJustificativeRequises.Delete(existing);
            await _uow.SaveChangesAsync();
            return NoContent();
        }
    }
}
