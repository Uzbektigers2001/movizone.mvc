using System;
using System.Collections.Generic;
using System.Linq;
using MovizoneApp.Models;

namespace MovizoneApp.Services
{
    public class ActorService : IActorService
    {
        private readonly List<Actor> _actors;

        public ActorService()
        {
            _actors = new List<Actor>
            {
                new Actor
                {
                    Id = 1,
                    Name = "Leonardo DiCaprio",
                    Bio = "Leonardo Wilhelm DiCaprio is an American actor and film producer. Known for his work in biopics and period films, he is the recipient of numerous accolades.",
                    BirthDate = new DateTime(1974, 11, 11),
                    Country = "USA",
                    Photo = "/img/covers/actor.jpg",
                    Movies = new List<string> { "Inception", "Titanic", "The Revenant" },
                    TVSeries = new List<string>()
                },
                new Actor
                {
                    Id = 2,
                    Name = "Tom Cruise",
                    Bio = "Thomas Cruise Mapother IV is an American actor and producer. One of the world's highest-paid actors, he has received various accolades.",
                    BirthDate = new DateTime(1962, 7, 3),
                    Country = "USA",
                    Photo = "/img/covers/actor.jpg",
                    Movies = new List<string> { "The Edge of Tomorrow", "Top Gun", "Mission: Impossible" },
                    TVSeries = new List<string>()
                },
                new Actor
                {
                    Id = 3,
                    Name = "Bryan Cranston",
                    Bio = "Bryan Lee Cranston is an American actor, director, producer, and screenwriter. He is best known for his roles as Walter White in Breaking Bad.",
                    BirthDate = new DateTime(1956, 3, 7),
                    Country = "USA",
                    Photo = "/img/covers/actor.jpg",
                    Movies = new List<string> { "Argo", "Godzilla" },
                    TVSeries = new List<string> { "Breaking Bad", "Malcolm in the Middle" }
                }
            };
        }

        public List<Actor> GetAllActors() => _actors;

        public Actor? GetActorById(int id) => _actors.FirstOrDefault(a => a.Id == id);

        public void AddActor(Actor actor)
        {
            actor.Id = _actors.Any() ? _actors.Max(a => a.Id) + 1 : 1;
            _actors.Add(actor);
        }

        public void UpdateActor(Actor actor)
        {
            var existingActor = GetActorById(actor.Id);
            if (existingActor != null)
            {
                existingActor.Name = actor.Name;
                existingActor.Bio = actor.Bio;
                existingActor.BirthDate = actor.BirthDate;
                existingActor.Country = actor.Country;
                existingActor.Photo = actor.Photo;
                existingActor.Movies = actor.Movies;
                existingActor.TVSeries = actor.TVSeries;
            }
        }

        public void DeleteActor(int id)
        {
            var actor = GetActorById(id);
            if (actor != null)
            {
                _actors.Remove(actor);
            }
        }
    }
}
