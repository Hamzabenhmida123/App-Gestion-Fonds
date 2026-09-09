using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FondsSocial.Application.DTOs;
using FondsSocial.Domain.Enums;

namespace FondsSocial.Application.Services
{
    public interface IDemandeService
    {
        Task<DemandeDto> CreateAsync(CreateDemandeDto dto);
        Task<DemandeDto?> GetByIdWithDetailsAsync(int id);
        /// <summary>
        /// Liste paginée et filtrée côté SQL (WHERE + Skip/Take exécutés par la base).
        /// </summary>
        Task<PagedResult<DemandeDto>> GetAllFilteredAsync(int? agentId, StatutDemande? statut, int? typeDePretId, DateTime? from, DateTime? to, int page, int pageSize);
        Task<IEnumerable<string>> AddPieceRecordAsync(CreatePieceJustificativeDto dto);
        Task<PieceJustificativeDto?> GetPieceByIdAsync(int pieceId);
        Task<bool> ChangePieceStatusAsync(int pieceId, StatutVerification statut, string auteur, string commentaire);
        Task<bool> TransitionStatutAsync(int demandeId, StatutDemande newStatut, string auteur, string commentaire);

        /// <summary>
        /// Clôture le dépôt d'une demande : vérifie que toutes les pièces obligatoires
        /// sont fournies et conformes, puis fait passer la demande au statut Enregistree.
        /// </summary>
        /// <returns>false si la demande n'existe pas ; lève InvalidOperationException si le dossier est incomplet.</returns>
        Task<bool> CloturerDepotAsync(int demandeId, string auteur, string commentaire);
    }
}
