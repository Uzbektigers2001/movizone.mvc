using System.Collections.Generic;
using System.Threading.Tasks;
using MovizoneApp.DTOs;

namespace MovizoneApp.Application.Interfaces
{
    /// <summary>
    /// Application service for actor-related business logic
    /// Now uses DTOs instead of exposing database models
    /// </summary>
    public interface IActorApplicationService
    {
        Task<IEnumerable<ActorDto>> GetAllActorsAsync();
        Task<ActorDto?> GetActorByIdAsync(int id);
        Task<ActorDto?> GetActorWithDetailsAsync(int id);
        Task<ActorDto> CreateActorAsync(CreateActorDto createActorDto);
        Task UpdateActorAsync(UpdateActorDto updateActorDto);
        Task DeleteActorAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}
