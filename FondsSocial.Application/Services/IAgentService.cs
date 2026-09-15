using System.Collections.Generic;
using System.Threading.Tasks;
using FondsSocial.Application.DTOs;

namespace FondsSocial.Application.Services
{
    public interface IAgentService
    {
        /// <summary>Liste paginée côté SQL (WHERE + Skip/Take exécutés par la base).</summary>
        Task<PagedResult<AgentDto>> GetAllAsync(int page = 1, int pageSize = 20);
        Task<AgentDto?> GetByIdAsync(int id);
        Task<AgentDto> CreateAsync(CreateAgentDto dto);
        Task<bool> UpdateAsync(int id, UpdateAgentDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
