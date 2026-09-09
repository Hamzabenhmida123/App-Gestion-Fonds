using System.Collections.Generic;
using System.Threading.Tasks;
using FondsSocial.Application.DTOs;

namespace FondsSocial.Application.Services
{
    public interface IParticipationSeanceService
    {
        Task<IEnumerable<ParticipationSeanceDto>> GetAllAsync();
        Task<ParticipationSeanceDto?> GetByIdAsync(int id);

        /// <summary>Vérifie que la séance et le membre existent avant d'enregistrer la participation.</summary>
        Task<ParticipationSeanceDto> CreateAsync(CreateParticipationSeanceDto dto);

        Task<bool> UpdateAsync(int id, UpdateParticipationSeanceDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
