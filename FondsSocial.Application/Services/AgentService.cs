using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using FondsSocial.Application.DTOs;
using FondsSocial.Domain.Entities;
using FondsSocial.Infrastructure.UnitOfWork;

namespace FondsSocial.Application.Services
{
    public class AgentService : IAgentService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public AgentService(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<AgentDto> CreateAsync(CreateAgentDto dto)
        {
            var entity = _mapper.Map<Agent>(dto);
            await _uow.Agents.AddAsync(entity);
            await _uow.SaveChangesAsync();
            return _mapper.Map<AgentDto>(entity);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _uow.Agents.GetByIdAsync(id);
            if (existing == null) return false;
            _uow.Agents.Delete(existing);
            await _uow.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<AgentDto>> GetAllAsync()
        {
            var list = await _uow.Agents.GetAllAsync();
            return _mapper.Map<IEnumerable<AgentDto>>(list);
        }

        public async Task<AgentDto?> GetByIdAsync(int id)
        {
            var e = await _uow.Agents.GetByIdAsync(id);
            if (e == null) return null;
            return _mapper.Map<AgentDto>(e);
        }

        public async Task<bool> UpdateAsync(int id, UpdateAgentDto dto)
        {
            var existing = await _uow.Agents.GetByIdAsync(id);
            if (existing == null) return false;
            _mapper.Map(dto, existing);
            _uow.Agents.Update(existing);
            await _uow.SaveChangesAsync();
            return true;
        }
    }
}
