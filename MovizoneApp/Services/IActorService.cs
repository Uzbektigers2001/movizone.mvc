using System.Collections.Generic;
using MovizoneApp.Models;

namespace MovizoneApp.Services
{
    public interface IActorService
    {
        List<Actor> GetAllActors();
        Actor? GetActorById(int id);
        void AddActor(Actor actor);
        void UpdateActor(Actor actor);
        void DeleteActor(int id);
    }
}
