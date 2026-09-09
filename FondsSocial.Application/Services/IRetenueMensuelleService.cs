using System.Collections.Generic;
using System.Threading.Tasks;
using FondsSocial.Application.DTOs;

namespace FondsSocial.Application.Services
{
    public interface IRetenueMensuelleService
    {
        Task<IEnumerable<RetenueMensuelleDto>> GetAllAsync();
        Task<RetenueMensuelleDto?> GetByIdAsync(int id);

        /// <summary>Vérifie que l'agent et le contrat existent avant d'enregistrer la retenue.</summary>
        Task<RetenueMensuelleDto> CreateAsync(CreateRetenueMensuelleDto dto);

        Task<bool> UpdateAsync(int id, UpdateRetenueMensuelleDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
