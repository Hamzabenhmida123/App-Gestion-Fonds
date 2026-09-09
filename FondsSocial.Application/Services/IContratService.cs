using System.Collections.Generic;
using System.Threading.Tasks;
using FondsSocial.Application.DTOs;

namespace FondsSocial.Application.Services
{
    public interface IContratService
    {
        Task<IEnumerable<ContratDto>> GetAllAsync();
        Task<ContratDto?> GetByIdAsync(int id);

        /// <summary>
        /// Crée un contrat pour une décision. Vérifie que la décision existe, qu'elle est
        /// favorable, et qu'aucun contrat n'existe déjà pour cette décision (relation 1:1).
        /// </summary>
        Task<ContratDto> CreateAsync(CreateContratDto dto);

        Task<bool> UpdateAsync(int id, UpdateContratDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
