using System.Collections.Generic;
using System.Threading.Tasks;
using FondsSocial.Application.DTOs;

namespace FondsSocial.Application.Services
{
    public interface IEcheanceService
    {
        /// <summary>Liste paginée côté SQL, filtrable par contrat.</summary>
        Task<PagedResult<EcheanceDto>> GetAllAsync(int? contratId = null, int page = 1, int pageSize = 20);
        Task<EcheanceDto?> GetByIdAsync(int id);

        /// <summary>Crée une échéance pour un contrat. Vérifie que le contrat existe.</summary>
        Task<EcheanceDto> CreateAsync(CreateEcheanceDto dto);

        Task<bool> UpdateAsync(int id, UpdateEcheanceDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
