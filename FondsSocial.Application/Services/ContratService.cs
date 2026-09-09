using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using FondsSocial.Application.DTOs;
using FondsSocial.Domain.Entities;
using FondsSocial.Domain.Enums;
using FondsSocial.Infrastructure.UnitOfWork;

namespace FondsSocial.Application.Services
{
    public class ContratService : IContratService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public ContratService(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ContratDto>> GetAllAsync()
        {
            var list = await _uow.Contrats.GetAllAsync();
            return _mapper.Map<IEnumerable<ContratDto>>(list);
        }

        public async Task<ContratDto?> GetByIdAsync(int id)
        {
            var e = await _uow.Contrats.GetByIdAsync(id);
            return e == null ? null : _mapper.Map<ContratDto>(e);
        }

        public async Task<ContratDto> CreateAsync(CreateContratDto dto)
        {
            var decision = await _uow.Decisions.GetByIdAsync(dto.DecisionId);
            if (decision == null) throw new System.InvalidOperationException("Décision introuvable");

            if (decision.SensDecision != SensDecision.Favorable)
                throw new System.InvalidOperationException("Un contrat ne peut être créé que pour une décision favorable.");

            var existing = await _uow.Contrats.FindAsync(c => c.DecisionId == dto.DecisionId);
            if (existing.Any())
                throw new System.InvalidOperationException("Un contrat existe déjà pour cette décision.");

            var entity = _mapper.Map<Contrat>(dto);
            await _uow.Contrats.AddAsync(entity);
            await _uow.SaveChangesAsync();
            return _mapper.Map<ContratDto>(entity);
        }

        public async Task<bool> UpdateAsync(int id, UpdateContratDto dto)
        {
            var existing = await _uow.Contrats.GetByIdAsync(id);
            if (existing == null) return false;
            _mapper.Map(dto, existing);
            _uow.Contrats.Update(existing);
            await _uow.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _uow.Contrats.GetByIdAsync(id);
            if (existing == null) return false;
            _uow.Contrats.Delete(existing);
            await _uow.SaveChangesAsync();
            return true;
        }
    }
}
