using System.Collections.Generic;
using System.Threading.Tasks;
using MovizoneApp.Models;

namespace MovizoneApp.Application.Interfaces
{
    /// <summary>
    /// Application service for actor-related business logic
    /// </summary>
    public interface IActorApplicationService
    {
        Task<List<Actor>> GetAllActorsAsync();
        Task<Actor?> GetActorByIdAsync(int id);
        Task<Actor?> GetActorWithDetailsAsync(int id);
        Task<Actor> CreateActorAsync(Actor actor);
        Task UpdateActorAsync(Actor actor);
        Task DeleteActorAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}
