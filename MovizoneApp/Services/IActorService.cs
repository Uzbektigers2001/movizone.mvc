using MovizoneApp.Models;

namespace MovizoneApp.Services
{
    public interface IActorService
    {
        List<Actor> GetAllActors();
        Actor? GetActorById(int id);
    }
}
