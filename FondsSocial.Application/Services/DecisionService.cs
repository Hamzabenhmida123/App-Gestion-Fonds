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
    public class DecisionService : IDecisionService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        private readonly IDemandeService _demandeService;

        public DecisionService(IUnitOfWork uow, IMapper mapper, IDemandeService demandeService)
        {
            _uow = uow;
            _mapper = mapper;
            _demandeService = demandeService;
        }

        public async Task<IEnumerable<DecisionDto>> GetAllAsync()
        {
            var list = await _uow.Decisions.GetAllAsync();
            return _mapper.Map<IEnumerable<DecisionDto>>(list);
        }

        public async Task<DecisionDto?> GetByIdAsync(int id)
        {
            var e = await _uow.Decisions.GetByIdAsync(id);
            return e == null ? null : _mapper.Map<DecisionDto>(e);
        }

        public async Task<DecisionDto> CreateAsync(CreateDecisionDto dto)
        {
            var demande = await _uow.Demandes.GetByIdAsync(dto.DemandeId);
            if (demande == null) throw new System.InvalidOperationException("Demande introuvable");

            var seance = await _uow.SeanceComites.GetByIdAsync(dto.SeanceComiteId);
            if (seance == null) throw new System.InvalidOperationException("Séance de comité introuvable");

            if (demande.StatutCourant != StatutDemande.AEtude && demande.StatutCourant != StatutDemande.PVVise)
                throw new System.InvalidOperationException(
                    $"Impossible d'enregistrer une décision: la demande doit être à l'étude (statut actuel: {demande.StatutCourant}).");

            var existing = await _uow.Decisions.FindAsync(d => d.DemandeId == dto.DemandeId);
            if (existing.Any(d => d.SensDecision == SensDecision.Favorable))
                throw new System.InvalidOperationException("Une décision favorable existe déjà pour cette demande.");

            var entity = _mapper.Map<Decision>(dto);
            await _uow.Decisions.AddAsync(entity);
            await _uow.SaveChangesAsync();

            // Une décision fait avancer la demande dans son cycle de vie (§13). Avant ce
            // correctif, enregistrer une décision n'avait aucun effet sur la demande: le comité
            // et le dossier restaient désynchronisés. On répercute maintenant le sens de la
            // décision sur le statut, avec traçabilité (auteur "Comité") dans l'historique.
            var targetStatut = dto.SensDecision switch
            {
                SensDecision.Favorable => (StatutDemande?)StatutDemande.PVVise,
                SensDecision.Defavorable => StatutDemande.Rejetee,
                SensDecision.Ajourne => StatutDemande.Ajournee,
                _ => null
            };
            if (targetStatut.HasValue)
            {
                await _demandeService.TransitionStatutAsync(demande.Id, targetStatut.Value, "Comité",
                    $"Décision {dto.SensDecision} enregistrée en séance de comité.");
            }

            return _mapper.Map<DecisionDto>(entity);
        }

        public async Task<bool> UpdateAsync(int id, UpdateDecisionDto dto)
        {
            var existing = await _uow.Decisions.GetByIdAsync(id);
            if (existing == null) return false;
            _mapper.Map(dto, existing);
            _uow.Decisions.Update(existing);
            await _uow.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _uow.Decisions.GetByIdAsync(id);
            if (existing == null) return false;
            _uow.Decisions.Delete(existing);
            await _uow.SaveChangesAsync();
            return true;
        }
    }
}
