using System.Collections.Generic;
using System.Threading.Tasks;
using FondsSocial.Application.DTOs;

namespace FondsSocial.Application.Services
{
    public interface IGarantieService
    {
        /// <summary>Liste paginée côté SQL, filtrable par contrat.</summary>
        Task<PagedResult<GarantieDto>> GetAllAsync(int? contratId = null, int page = 1, int pageSize = 20);
        Task<GarantieDto?> GetByIdAsync(int id);

        /// <summary>Vérifie que le contrat existe et n'a pas déjà de garantie (relation 1:1).</summary>
        Task<GarantieDto> CreateAsync(CreateGarantieDto dto);

        Task<bool> UpdateAsync(int id, UpdateGarantieDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
