using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using FondsSocial.Application.DTOs;
using FondsSocial.Domain.Entities;
using FondsSocial.Infrastructure.UnitOfWork;

namespace FondsSocial.Application.Services
{
    public class ParticipationSeanceService : IParticipationSeanceService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public ParticipationSeanceService(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ParticipationSeanceDto>> GetAllAsync()
        {
            var list = await _uow.ParticipationSeances.GetAllAsync();
            return _mapper.Map<IEnumerable<ParticipationSeanceDto>>(list);
        }

        public async Task<ParticipationSeanceDto?> GetByIdAsync(int id)
        {
            var e = await _uow.ParticipationSeances.GetByIdAsync(id);
            return e == null ? null : _mapper.Map<ParticipationSeanceDto>(e);
        }

        public async Task<ParticipationSeanceDto> CreateAsync(CreateParticipationSeanceDto dto)
        {
            var seance = await _uow.SeanceComites.GetByIdAsync(dto.SeanceComiteId);
            if (seance == null) throw new System.InvalidOperationException("Séance de comité introuvable");

            var membre = await _uow.MembreComites.GetByIdAsync(dto.MembreComiteId);
            if (membre == null) throw new System.InvalidOperationException("Membre du comité introuvable");

            var entity = _mapper.Map<ParticipationSeance>(dto);
            await _uow.ParticipationSeances.AddAsync(entity);
            await _uow.SaveChangesAsync();
            return _mapper.Map<ParticipationSeanceDto>(entity);
        }

        public async Task<bool> UpdateAsync(int id, UpdateParticipationSeanceDto dto)
        {
            var existing = await _uow.ParticipationSeances.GetByIdAsync(id);
            if (existing == null) return false;
            _mapper.Map(dto, existing);
            _uow.ParticipationSeances.Update(existing);
            await _uow.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _uow.ParticipationSeances.GetByIdAsync(id);
            if (existing == null) return false;
            _uow.ParticipationSeances.Delete(existing);
            await _uow.SaveChangesAsync();
            return true;
        }
    }
}
