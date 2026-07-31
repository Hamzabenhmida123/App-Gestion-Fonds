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

            // --- Eligibility checks (§5) ---

            // 1) Ancienneté minimale : 1 an de titularisation
            if (!agent.DateTitularisation.HasValue)
                throw new InvalidOperationException("Date de titularisation non renseignée: ancienneté minimum d'un an requise.");
            if (dto.DateDepot < agent.DateTitularisation.Value.AddYears(1))
                throw new InvalidOperationException("Ancienneté minimale d'un an non respectée (date de titularisation requise).");

            // 2) Plafond du type de prêt
            if (dto.MontantDemande > type.Plafond)
                throw new InvalidOperationException($"Montant demandé ({dto.MontantDemande}) dépasse le plafond ({type.Plafond}) pour ce type de prêt.");

            // 3) Franchise : attendre FranchiseMois entre deux demandes du même type.
            //    Les demandes rejetées ou caduques ne comptent pas dans la franchise.
            var previous = (await _uow.Demandes.FindAsync(d => d.AgentId == dto.AgentId
                    && d.TypeDePretId == dto.TypeDePretId
                    && d.StatutCourant != StatutDemande.Rejetee
                    && d.StatutCourant != StatutDemande.Caduque))
                .OrderByDescending(d => d.DateDepot)
                .FirstOrDefault();

            if (previous != null)
            {
                var months = (dto.DateDepot.Year - previous.DateDepot.Year) * 12 + dto.DateDepot.Month - previous.DateDepot.Month;
                if (months < type.FranchiseMois)
                    throw new InvalidOperationException($"Franchise non respectée: il faut attendre {type.FranchiseMois} mois depuis la dernière demande du même type.");
            }

            // 4) Taux d'endettement ≤ 40 %
            if (!agent.SalaireMensuel.HasValue || agent.SalaireMensuel.Value <= 0)
                throw new InvalidOperationException("Salaire mensuel non renseigné: impossible de calculer le taux d'endettement.");
            if (type.DureeMaxMois <= 0)
                throw new InvalidOperationException("Durée maximale du prêt invalide: impossible d'estimer la mensualité.");

            var retenuesEnCours = await _uow.RetenuesMensuelles.FindAsync(
                r => r.AgentId == agent.Id && (r.Statut == StatutRetenue.Retenue || r.Statut == StatutRetenue.Prevue));
            var mensualitesExistantes = retenuesEnCours.Sum(r => r.MontantARetenir);
            var nouvelleMensualiteEstimee = dto.MontantDemande / type.DureeMaxMois;
            var tauxEndettement = (mensualitesExistantes + nouvelleMensualiteEstimee) / agent.SalaireMensuel.Value;
            if (tauxEndettement > 0.40m)
                throw new InvalidOperationException($"Taux d'endettement ({tauxEndettement:P0}) supérieur au plafond de 40%.");

            // 5) Plafond cumulé « logement » : 30 000 DT, tous types confondus
            if (type.Categorie == CategorieBudget.Logement)
            {
                const decimal plafondCumuleLogement = 30000m;

                var typesLogementIds = (await _uow.TypeDePrets.GetAllAsync())
                    .Where(t => t.Categorie == CategorieBudget.Logement)
                    .Select(t => t.Id)
                    .ToHashSet();

                var demandesLogement = (await _uow.Demandes.FindAsync(d => d.AgentId == agent.Id))
                    .Where(d => typesLogementIds.Contains(d.TypeDePretId))
                    .ToList();

                var demandeMontantMap = demandesLogement.ToDictionary(d => d.Id, d => d.MontantDemande);
                var idsDemandesLogement = demandesLogement.Select(d => d.Id).ToList();

                var decisionsLogement = idsDemandesLogement.Count > 0
                    ? await _uow.Decisions.FindAsync(dec => idsDemandesLogement.Contains(dec.DemandeId) && dec.SensDecision == SensDecision.Favorable)
                    : new List<Decision>();

                var totalEngage = decisionsLogement.Sum(dec => dec.MontantAccorde ?? (demandeMontantMap.TryGetValue(dec.DemandeId, out var m) ? m : 0m));
                totalEngage += dto.MontantDemande;

                if (totalEngage > plafondCumuleLogement)
                    throw new InvalidOperationException($"Plafond cumulé logement dépassé: {totalEngage} DT engagés sur un maximum de {plafondCumuleLogement} DT.");
            }

            var entity = _mapper.Map<Demande>(dto);
            entity.NumeroDossier = GenerateNumeroDossier();
            entity.StatutCourant = StatutDemande.Deposee;
            entity.ScorePriorite = CalculateScorePriorite(agent, dto.DateDepot);

            // Attach the initial history entry via the navigation collection
            // so EF Core sets DemandeId automatically after the parent insert.
            entity.HistoriqueStatuts.Add(new HistoriqueStatutDemande
            {
                Statut = StatutDemande.Deposee,
                DateChangement = DateTime.UtcNow,
                Auteur = "System",
                Commentaire = "Demande déposée"
            });

            await _uow.Demandes.AddAsync(entity);
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

        public async Task<bool> CloturerDepotAsync(int demandeId, string auteur, string commentaire)
        {
            var demande = await _uow.Demandes.GetByIdAsync(demandeId);
            if (demande == null) return false;

            // Seules les demandes déposées peuvent être clôturées
            if (demande.StatutCourant != StatutDemande.Deposee)
                throw new InvalidOperationException($"Clôture impossible: la demande est au statut {demande.StatutCourant}.");

            // Pièces requises pour le type de prêt
            var requises = await _uow.PieceJustificativeRequises.FindAsync(r => r.TypeDePretId == demande.TypeDePretId);
            var fournies = await _uow.PieceJustificatives.FindAsync(p => p.DemandeId == demandeId);

            var manquantes = requises
                .Where(r => r.Obligatoire)
                .Select(r => r.LibellePiece)
                .Except(fournies.Select(p => p.TypePiece))
                .ToList();

            if (manquantes.Count > 0)
                throw new InvalidOperationException($"Dossier incomplet: pièces obligatoires manquantes: {string.Join(", ", manquantes)}.");

            // Pièces fournies mais non conformes
            var nonConformes = fournies
                .Where(p => p.StatutVerification == StatutVerification.NonConforme)
                .Select(p => p.TypePiece)
                .ToList();

            if (nonConformes.Count > 0)
                throw new InvalidOperationException($"Dossier incomplet: pièces non conformes: {string.Join(", ", nonConformes)}.");

            // Tout est complet et conforme → enregistrer la demande
            demande.StatutCourant = StatutDemande.Enregistree;
            _uow.Demandes.Update(demande);

            var hist = new HistoriqueStatutDemande
            {
                DemandeId = demande.Id,
                Statut = StatutDemande.Enregistree,
                DateChangement = DateTime.UtcNow,
                Auteur = auteur,
                Commentaire = string.IsNullOrWhiteSpace(commentaire) ? "Dépôt clôturé: dossier complet" : commentaire
            };
            await _uow.HistoriqueStatutDemandes.AddAsync(hist);

            await _uow.SaveChangesAsync();
            return true;
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

        /// <summary>
        /// Calcule le score de priorité (§5) :
        /// - 4 pts par année d'ancienneté (année complète),
        /// - 4 pts si marié,
        /// - 4 pts si divorcé/veuf avec enfants à charge (garde d'enfants),
        /// - 2 pts par enfant à charge.
        /// </summary>
        private decimal CalculateScorePriorite(Agent agent, DateTime dateDepot)
        {
            decimal score = 0;

            // 4 pts par année d'ancienneté complète
            if (agent.DateTitularisation.HasValue)
            {
                var mois = (dateDepot.Year - agent.DateTitularisation.Value.Year) * 12 + dateDepot.Month - agent.DateTitularisation.Value.Month;
                score += 4m * Math.Floor(mois / 12m);
            }

            // 4 pts si marié
            if (agent.SituationFamiliale == SituationFamiliale.Marie)
                score += 4m;

            // 4 pts si divorcé/veuf avec enfants à charge (garde d'enfants)
            if ((agent.SituationFamiliale == SituationFamiliale.Divorce || agent.SituationFamiliale == SituationFamiliale.Veuf)
                && agent.NombreEnfantsACharge > 0)
                score += 4m;

            // 2 pts par enfant à charge
            score += 2m * agent.NombreEnfantsACharge;

            return score;
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
