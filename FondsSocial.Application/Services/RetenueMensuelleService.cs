using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using FondsSocial.Application.DTOs;
using FondsSocial.Domain.Entities;
using FondsSocial.Infrastructure.UnitOfWork;

namespace FondsSocial.Application.Services
{
    public class RetenueMensuelleService : IRetenueMensuelleService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public RetenueMensuelleService(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<PagedResult<RetenueMensuelleDto>> GetAllAsync(int? contratId = null, int page = 1, int pageSize = 20)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 20;
            if (pageSize > 100) pageSize = 100;

            System.Linq.Expressions.Expression<Func<RetenueMensuelle, bool>>? filter = contratId.HasValue
                ? r => r.ContratId == contratId.Value
                : null;

            var (items, totalCount) = await _uow.RetenuesMensuelles.GetPagedAsync(page, pageSize, filter);
            return new PagedResult<RetenueMensuelleDto>
            {
                Items = _mapper.Map<IEnumerable<RetenueMensuelleDto>>(items),
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<RetenueMensuelleDto?> GetByIdAsync(int id)
        {
            var e = await _uow.RetenuesMensuelles.GetByIdAsync(id);
            return e == null ? null : _mapper.Map<RetenueMensuelleDto>(e);
        }

        public async Task<RetenueMensuelleDto> CreateAsync(CreateRetenueMensuelleDto dto)
        {
            var agent = await _uow.Agents.GetByIdAsync(dto.AgentId);
            if (agent == null) throw new System.InvalidOperationException("Agent introuvable");

            var contrat = await _uow.Contrats.GetByIdAsync(dto.ContratId);
            if (contrat == null) throw new System.InvalidOperationException("Contrat introuvable");

            var entity = _mapper.Map<RetenueMensuelle>(dto);
            await _uow.RetenuesMensuelles.AddAsync(entity);
            await _uow.SaveChangesAsync();
            return _mapper.Map<RetenueMensuelleDto>(entity);
        }

        public async Task<bool> UpdateAsync(int id, UpdateRetenueMensuelleDto dto)
        {
            var existing = await _uow.RetenuesMensuelles.GetByIdAsync(id);
            if (existing == null) return false;
            _mapper.Map(dto, existing);
            _uow.RetenuesMensuelles.Update(existing);
            await _uow.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _uow.RetenuesMensuelles.GetByIdAsync(id);
            if (existing == null) return false;
            _uow.RetenuesMensuelles.Delete(existing);
            await _uow.SaveChangesAsync();
            return true;
        }
    }
}
