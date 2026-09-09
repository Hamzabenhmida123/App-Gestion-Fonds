using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using FondsSocial.Application.DTOs;
using FondsSocial.Domain.Entities;
using FondsSocial.Infrastructure.UnitOfWork;

namespace FondsSocial.Application.Services
{
    public class EcheanceService : IEcheanceService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public EcheanceService(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<IEnumerable<EcheanceDto>> GetAllAsync()
        {
            var list = await _uow.Echeances.GetAllAsync();
            return _mapper.Map<IEnumerable<EcheanceDto>>(list);
        }

        public async Task<EcheanceDto?> GetByIdAsync(int id)
        {
            var e = await _uow.Echeances.GetByIdAsync(id);
            return e == null ? null : _mapper.Map<EcheanceDto>(e);
        }

        public async Task<EcheanceDto> CreateAsync(CreateEcheanceDto dto)
        {
            var contrat = await _uow.Contrats.GetByIdAsync(dto.ContratId);
            if (contrat == null) throw new System.InvalidOperationException("Contrat introuvable");

            var entity = _mapper.Map<Echeance>(dto);
            await _uow.Echeances.AddAsync(entity);
            await _uow.SaveChangesAsync();
            return _mapper.Map<EcheanceDto>(entity);
        }

        public async Task<bool> UpdateAsync(int id, UpdateEcheanceDto dto)
        {
            var existing = await _uow.Echeances.GetByIdAsync(id);
            if (existing == null) return false;
            _mapper.Map(dto, existing);
            _uow.Echeances.Update(existing);
            await _uow.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _uow.Echeances.GetByIdAsync(id);
            if (existing == null) return false;
            _uow.Echeances.Delete(existing);
            await _uow.SaveChangesAsync();
            return true;
        }
    }
}
