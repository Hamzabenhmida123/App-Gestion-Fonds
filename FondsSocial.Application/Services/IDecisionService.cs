using System.Collections.Generic;
using System.Threading.Tasks;
using FondsSocial.Application.DTOs;

namespace FondsSocial.Application.Services
{
    public interface IDecisionService
    {
        Task<IEnumerable<DecisionDto>> GetAllAsync();
        Task<DecisionDto?> GetByIdAsync(int id);

        /// <summary>
        /// Enregistre une décision de comité pour une demande. Vérifie que la demande et la
        /// séance existent, que la demande est bien à l'étude, et qu'aucune décision favorable
        /// n'a déjà été rendue pour ce dossier. Répercute automatiquement le sens de la décision
        /// sur le statut de la demande (Favorable -> PVVise, Défavorable -> Rejetée,
        /// Ajournée -> Ajournée) via IDemandeService, avec traçabilité dans l'historique.
        /// </summary>
        Task<DecisionDto> CreateAsync(CreateDecisionDto dto);

        Task<bool> UpdateAsync(int id, UpdateDecisionDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
