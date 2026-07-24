using System.Collections.Generic;
using System.Threading.Tasks;
using FondsSocial.Application.DTOs;

namespace FondsSocial.Application.Services
{
    public interface IAgentService
    {
        Task<IEnumerable<AgentDto>> GetAllAsync();
        Task<AgentDto?> GetByIdAsync(int id);
        Task<AgentDto> CreateAsync(CreateAgentDto dto);
        Task<bool> UpdateAsync(int id, UpdateAgentDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
