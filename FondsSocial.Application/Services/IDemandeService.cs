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
        Task<IEnumerable<DemandeDto>> GetAllFilteredAsync(int? agentId, StatutDemande? statut, int? typeDePretId, DateTime? from, DateTime? to);
        Task<IEnumerable<string>> AddPieceRecordAsync(CreatePieceJustificativeDto dto);
        Task<bool> ChangePieceStatusAsync(int pieceId, StatutVerification statut, string auteur, string commentaire);
        Task<bool> TransitionStatutAsync(int demandeId, StatutDemande newStatut, string auteur, string commentaire);
    }
}
