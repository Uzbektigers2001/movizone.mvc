using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MovizoneApp.Application.Interfaces;
using MovizoneApp.Core.Exceptions;
using MovizoneApp.Core.Interfaces;
using MovizoneApp.Models;

namespace MovizoneApp.Application.Services
{
    /// <summary>
    /// Application service for actor business logic
    /// </summary>
    public class ActorApplicationService : IActorApplicationService
    {
        private readonly IActorRepository _actorRepository;
        private readonly ILogger<ActorApplicationService> _logger;

        public ActorApplicationService(
            IActorRepository actorRepository,
            ILogger<ActorApplicationService> logger)
        {
            _actorRepository = actorRepository;
            _logger = logger;
        }

        public async Task<List<Actor>> GetAllActorsAsync()
        {
            _logger.LogInformation("Fetching all actors");
            var actors = await _actorRepository.GetAllAsync();

            return actors.ToList();
        }

        public async Task<Actor?> GetActorByIdAsync(int id)
        {
            _logger.LogInformation("Fetching actor with ID: {ActorId}", id);
            var actor = await _actorRepository.GetByIdAsync(id);

            if (actor == null)
            {
                _logger.LogWarning("Actor with ID {ActorId} not found", id);
            }

            return actor;
        }

        public async Task<Actor?> GetActorWithDetailsAsync(int id)
        {
            _logger.LogInformation("Fetching actor with details, ID: {ActorId}", id);
            return await _actorRepository.GetActorWithDetailsAsync(id);
        }

        public async Task<Actor> CreateActorAsync(Actor actor)
        {
            _logger.LogInformation("Creating new actor: {ActorName}", actor.Name);

            // Business validation
            if (string.IsNullOrWhiteSpace(actor.Name))
            {
                throw new BadRequestException("Actor name is required");
            }

            if (actor.BirthDate > DateTime.UtcNow)
            {
                throw new BadRequestException("Birth date cannot be in the future");
            }

            var age = DateTime.UtcNow.Year - actor.BirthDate.Year;
            if (age > 150)
            {
                throw new BadRequestException("Invalid birth date");
            }

            actor.CreatedAt = DateTime.UtcNow;
            var created = await _actorRepository.AddAsync(actor);

            _logger.LogInformation("Actor created successfully with ID: {ActorId}", created.Id);
            return created;
        }

        public async Task UpdateActorAsync(Actor actor)
        {
            _logger.LogInformation("Updating actor with ID: {ActorId}", actor.Id);

            var existing = await _actorRepository.GetByIdAsync(actor.Id);
            if (existing == null)
            {
                throw new NotFoundException("Actor", actor.Id);
            }

            // Business validation
            if (string.IsNullOrWhiteSpace(actor.Name))
            {
                throw new BadRequestException("Actor name is required");
            }

            actor.UpdatedAt = DateTime.UtcNow;
            actor.CreatedAt = existing.CreatedAt;

            await _actorRepository.UpdateAsync(actor);
            _logger.LogInformation("Actor updated successfully: {ActorId}", actor.Id);
        }

        public async Task DeleteActorAsync(int id)
        {
            _logger.LogInformation("Deleting actor with ID: {ActorId}", id);

            var actor = await _actorRepository.GetByIdAsync(id);
            if (actor == null)
            {
                throw new NotFoundException("Actor", id);
            }

            // Soft delete
            actor.IsDeleted = true;
            actor.UpdatedAt = DateTime.UtcNow;
            await _actorRepository.UpdateAsync(actor);

            _logger.LogInformation("Actor soft-deleted successfully: {ActorId}", id);
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _actorRepository.ExistsAsync(a => a.Id == id);
        }
    }
}
