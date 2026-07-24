using System.Collections.Generic;
using System.Threading.Tasks;
using FondsSocial.Application.DTOs;

namespace FondsSocial.Application.Services
{
    public interface ITypeDePretService
    {
        Task<IEnumerable<TypeDePretDto>> GetAllAsync();
        Task<TypeDePretDto?> GetByIdAsync(int id);
        Task<TypeDePretDto> CreateAsync(CreateTypeDePretDto dto);
        Task<bool> UpdateAsync(int id, UpdateTypeDePretDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
