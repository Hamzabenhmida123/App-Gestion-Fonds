using System.Collections.Generic;
using System.Threading.Tasks;
using FondsSocial.Application.DTOs;

namespace FondsSocial.Application.Services
{
    public interface IRetenueMensuelleService
    {
        /// <summary>Liste paginée côté SQL, filtrable par contrat.</summary>
        Task<PagedResult<RetenueMensuelleDto>> GetAllAsync(int? contratId = null, int page = 1, int pageSize = 20);
        Task<RetenueMensuelleDto?> GetByIdAsync(int id);

        /// <summary>Vérifie que l'agent et le contrat existent avant d'enregistrer la retenue.</summary>
        Task<RetenueMensuelleDto> CreateAsync(CreateRetenueMensuelleDto dto);

        Task<bool> UpdateAsync(int id, UpdateRetenueMensuelleDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
