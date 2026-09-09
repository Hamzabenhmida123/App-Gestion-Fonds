using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using FondsSocial.Application.DTOs;
using FondsSocial.Domain.Entities;
using FondsSocial.Infrastructure.UnitOfWork;

namespace FondsSocial.Application.Services
{
    public class GarantieService : IGarantieService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public GarantieService(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<IEnumerable<GarantieDto>> GetAllAsync()
        {
            var list = await _uow.Garanties.GetAllAsync();
            return _mapper.Map<IEnumerable<GarantieDto>>(list);
        }

        public async Task<GarantieDto?> GetByIdAsync(int id)
        {
            var e = await _uow.Garanties.GetByIdAsync(id);
            return e == null ? null : _mapper.Map<GarantieDto>(e);
        }

        public async Task<GarantieDto> CreateAsync(CreateGarantieDto dto)
        {
            var contrat = await _uow.Contrats.GetByIdAsync(dto.ContratId);
            if (contrat == null) throw new System.InvalidOperationException("Contrat introuvable");

            var existing = await _uow.Garanties.FindAsync(g => g.ContratId == dto.ContratId);
            if (existing.Any())
                throw new System.InvalidOperationException("Une garantie existe déjà pour ce contrat.");

            var entity = _mapper.Map<Garantie>(dto);
            await _uow.Garanties.AddAsync(entity);
            await _uow.SaveChangesAsync();
            return _mapper.Map<GarantieDto>(entity);
        }

        public async Task<bool> UpdateAsync(int id, UpdateGarantieDto dto)
        {
            var existing = await _uow.Garanties.GetByIdAsync(id);
            if (existing == null) return false;
            _mapper.Map(dto, existing);
            _uow.Garanties.Update(existing);
            await _uow.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _uow.Garanties.GetByIdAsync(id);
            if (existing == null) return false;
            _uow.Garanties.Delete(existing);
            await _uow.SaveChangesAsync();
            return true;
        }
    }
}
