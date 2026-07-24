using System;
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
    public class DemandeService : IDemandeService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public DemandeService(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<DemandeDto> CreateAsync(CreateDemandeDto dto)
        {
            // verify agent and type
            var agent = await _uow.Agents.GetByIdAsync(dto.AgentId);
            if (agent == null) throw new InvalidOperationException("Agent introuvable");

            var type = await _uow.TypeDePrets.GetByIdAsync(dto.TypeDePretId);
            if (type == null) throw new InvalidOperationException("Type de prêt introuvable");

            // eligibility checks
            if (dto.MontantDemande > type.Plafond)
                throw new InvalidOperationException($"Montant demandé ({dto.MontantDemande}) dépasse le plafond ({type.Plafond}) pour ce type de prêt.");

            // franchise: vérifier la dernière demande de cet agent pour ce type
            var previous = (await _uow.Demandes.FindAsync(d => d.AgentId == dto.AgentId && d.TypeDePretId == dto.TypeDePretId))
                .OrderByDescending(d => d.DateDepot)
                .FirstOrDefault();

            if (previous != null)
            {
                var months = (dto.DateDepot.Year - previous.DateDepot.Year) * 12 + dto.DateDepot.Month - previous.DateDepot.Month;
                if (months < type.FranchiseMois)
                    throw new InvalidOperationException($"Franchise non respectée: il faut attendre {type.FranchiseMois} mois depuis la dernière demande du même type.");
            }

            var entity = _mapper.Map<Demande>(dto);
            entity.NumeroDossier = GenerateNumeroDossier();
            entity.StatutCourant = StatutDemande.Deposee;
            entity.ScorePriorite = 0;

            await _uow.Demandes.AddAsync(entity);

            // add initial historique
            var hist = new HistoriqueStatutDemande
            {
                DemandeId = entity.Id,
                Statut = StatutDemande.Deposee,
                DateChangement = DateTime.UtcNow,
                Auteur = "System",
                Commentaire = "Demande déposée"
            };
            await _uow.HistoriqueStatutDemandes.AddAsync(hist);

            await _uow.SaveChangesAsync();

            return _mapper.Map<DemandeDto>(entity);
        }

        private string GenerateNumeroDossier()
        {
            return $"D-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString().Split('-')[0].ToUpper()}";
        }

        public async Task<DemandeDto?> GetByIdWithDetailsAsync(int id)
        {
            var e = await _uow.Demandes.GetByIdAsync(id);
            if (e == null) return null;
            // load related collections via repositories (simple approach: find by predicate)
            var hist = await _uow.HistoriqueStatutDemandes.FindAsync(h => h.DemandeId == id);
            var pieces = await _uow.PieceJustificatives.FindAsync(p => p.DemandeId == id);
            var decisions = await _uow.Decisions.FindAsync(d => d.DemandeId == id);

            var dto = _mapper.Map<DemandeDto>(e);
            dto.HistoriqueStatuts = _mapper.Map<IEnumerable<HistoriqueStatutDemandeDto>>(hist);
            dto.PieceJustificatives = _mapper.Map<IEnumerable<PieceJustificativeDto>>(pieces);
            dto.Decisions = _mapper.Map<IEnumerable<DecisionDto>>(decisions);
            return dto;
        }

        public async Task<IEnumerable<DemandeDto>> GetAllFilteredAsync(int? agentId, StatutDemande? statut, int? typeDePretId, DateTime? from, DateTime? to)
        {
            var list = await _uow.Demandes.GetAllAsync();
            var q = list.AsQueryable();
            if (agentId.HasValue) q = q.Where(d => d.AgentId == agentId.Value);
            if (statut.HasValue) q = q.Where(d => d.StatutCourant == statut.Value);
            if (typeDePretId.HasValue) q = q.Where(d => d.TypeDePretId == typeDePretId.Value);
            if (from.HasValue) q = q.Where(d => d.DateDepot >= from.Value);
            if (to.HasValue) q = q.Where(d => d.DateDepot <= to.Value);

            return _mapper.Map<IEnumerable<DemandeDto>>(q.OrderByDescending(d => d.DateDepot));
        }

        public async Task<IEnumerable<string>> AddPieceRecordAsync(CreatePieceJustificativeDto dto)
        {
            var demande = await _uow.Demandes.GetByIdAsync(dto.DemandeId);
            if (demande == null) throw new InvalidOperationException("Demande introuvable");

            var piece = _mapper.Map<PieceJustificative>(dto);
            piece.DateDepot = DateTime.UtcNow;
            piece.StatutVerification = StatutVerification.EnAttente;
            await _uow.PieceJustificatives.AddAsync(piece);
            await _uow.SaveChangesAsync();

            // Check required pieces for the type
            var required = await _uow.PieceJustificativeRequises.FindAsync(r => r.TypeDePretId == demande.TypeDePretId);
            var provided = await _uow.PieceJustificatives.FindAsync(p => p.DemandeId == dto.DemandeId);

            var missing = required.Select(r => r.LibellePiece).Except(provided.Select(p => p.TypePiece)).ToList();
            return missing;
        }

        public async Task<bool> ChangePieceStatusAsync(int pieceId, StatutVerification statut, string auteur, string commentaire)
        {
            var piece = await _uow.PieceJustificatives.GetByIdAsync(pieceId);
            if (piece == null) return false;
            piece.StatutVerification = statut;
            _uow.PieceJustificatives.Update(piece);

            // add history entry to demande
            var hist = new HistoriqueStatutDemande
            {
                DemandeId = piece.DemandeId,
                Statut = StatutDemande.AEtude,
                DateChangement = DateTime.UtcNow,
                Auteur = auteur,
                Commentaire = commentaire
            };
            await _uow.HistoriqueStatutDemandes.AddAsync(hist);

            await _uow.SaveChangesAsync();
            return true;
        }

        public async Task<bool> TransitionStatutAsync(int demandeId, StatutDemande newStatut, string auteur, string commentaire)
        {
            var demande = await _uow.Demandes.GetByIdAsync(demandeId);
            if (demande == null) return false;
            if (!IsValidTransition(demande.StatutCourant, newStatut)) return false;

            demande.StatutCourant = newStatut;
            _uow.Demandes.Update(demande);

            var hist = new HistoriqueStatutDemande
            {
                DemandeId = demande.Id,
                Statut = newStatut,
                DateChangement = DateTime.UtcNow,
                Auteur = auteur,
                Commentaire = commentaire
            };
            await _uow.HistoriqueStatutDemandes.AddAsync(hist);
            await _uow.SaveChangesAsync();
            return true;
        }

        private bool IsValidTransition(StatutDemande current, StatutDemande target)
        {
            // simple allowed transitions
            var map = new Dictionary<StatutDemande, StatutDemande[]>
            {
                { StatutDemande.Deposee, new[]{ StatutDemande.Enregistree, StatutDemande.Rejetee, StatutDemande.AEtude } },
                { StatutDemande.Enregistree, new[]{ StatutDemande.AEtude, StatutDemande.Rejetee } },
                { StatutDemande.AEtude, new[]{ StatutDemande.PVVise, StatutDemande.Ajournee, StatutDemande.Rejetee } },
                { StatutDemande.PVVise, new[]{ StatutDemande.Signee, StatutDemande.Rejetee } },
                { StatutDemande.Signee, new[]{ StatutDemande.DecisionNotifiee } },
                { StatutDemande.DecisionNotifiee, new[]{ StatutDemande.RetenueProgrammee, StatutDemande.DecaisseeCloturee } },
            };

            if (current == target) return false;
            if (!map.ContainsKey(current)) return false;
            return map[current].Contains(target);
        }
    }
}
