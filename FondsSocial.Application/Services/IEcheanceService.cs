using System.Collections.Generic;
using System.Threading.Tasks;
using FondsSocial.Application.DTOs;

namespace FondsSocial.Application.Services
{
    public interface IEcheanceService
    {
        Task<IEnumerable<EcheanceDto>> GetAllAsync();
        Task<EcheanceDto?> GetByIdAsync(int id);

        /// <summary>Crée une échéance pour un contrat. Vérifie que le contrat existe.</summary>
        Task<EcheanceDto> CreateAsync(CreateEcheanceDto dto);

        Task<bool> UpdateAsync(int id, UpdateEcheanceDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
