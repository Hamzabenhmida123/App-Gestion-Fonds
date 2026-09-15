using System.Collections.Generic;
using System.Threading.Tasks;
using FondsSocial.Application.DTOs;

namespace FondsSocial.Application.Services
{
    public interface IParticipationSeanceService
    {
        /// <summary>Liste paginée côté SQL, filtrable par séance de comité.</summary>
        Task<PagedResult<ParticipationSeanceDto>> GetAllAsync(int? seanceComiteId = null, int page = 1, int pageSize = 20);
        Task<ParticipationSeanceDto?> GetByIdAsync(int id);

        /// <summary>Vérifie que la séance et le membre existent avant d'enregistrer la participation.</summary>
        Task<ParticipationSeanceDto> CreateAsync(CreateParticipationSeanceDto dto);

        Task<bool> UpdateAsync(int id, UpdateParticipationSeanceDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
