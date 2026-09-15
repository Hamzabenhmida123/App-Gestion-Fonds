using System.Collections.Generic;
using System.Threading.Tasks;
using FondsSocial.Application.DTOs;

namespace FondsSocial.Application.Services
{
    public interface ITypeDePretService
    {
        /// <summary>Liste paginée côté SQL (WHERE + Skip/Take exécutés par la base).</summary>
        Task<PagedResult<TypeDePretDto>> GetAllAsync(int page = 1, int pageSize = 20);
        Task<TypeDePretDto?> GetByIdAsync(int id);
        Task<TypeDePretDto> CreateAsync(CreateTypeDePretDto dto);
        Task<bool> UpdateAsync(int id, UpdateTypeDePretDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
