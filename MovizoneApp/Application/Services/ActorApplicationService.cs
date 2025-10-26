using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Logging;
using MovizoneApp.Application.Interfaces;
using MovizoneApp.Core.Exceptions;
using MovizoneApp.Core.Interfaces;
using MovizoneApp.DTOs;
using MovizoneApp.Models;

namespace MovizoneApp.Application.Services
{
    /// <summary>
    /// Application service for actor business logic
    /// Uses DTOs and AutoMapper for clean separation from database models
    /// </summary>
    public class ActorApplicationService : IActorApplicationService
    {
        private readonly IActorRepository _actorRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<ActorApplicationService> _logger;

        public ActorApplicationService(
            IActorRepository actorRepository,
            IMapper mapper,
            ILogger<ActorApplicationService> logger)
        {
            _actorRepository = actorRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<ActorDto>> GetAllActorsAsync()
        {
            _logger.LogInformation("Fetching all actors");
            var actors = await _actorRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<ActorDto>>(actors);
        }

        public async Task<ActorDto?> GetActorByIdAsync(int id)
        {
            _logger.LogInformation("Fetching actor with ID: {ActorId}", id);
            var actor = await _actorRepository.GetByIdAsync(id);

            if (actor == null)
            {
                _logger.LogWarning("Actor with ID {ActorId} not found", id);
                return null;
            }

            return _mapper.Map<ActorDto>(actor);
        }

        public async Task<ActorDto?> GetActorWithDetailsAsync(int id)
        {
            _logger.LogInformation("Fetching actor with details, ID: {ActorId}", id);
            var actor = await _actorRepository.GetActorWithDetailsAsync(id);
            return actor == null ? null : _mapper.Map<ActorDto>(actor);
        }

        public async Task<ActorDto> CreateActorAsync(CreateActorDto createActorDto)
        {
            _logger.LogInformation("Creating new actor: {ActorName}", createActorDto.Name);

            // Map DTO to Model
            var actor = _mapper.Map<Actor>(createActorDto);

            // Business validation (additional to DTO validation)
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

            // Set timestamps
            actor.CreatedAt = DateTime.UtcNow;

            // Save to repository
            var created = await _actorRepository.AddAsync(actor);

            _logger.LogInformation("Actor created successfully with ID: {ActorId}", created.Id);

            // Map back to DTO and return
            return _mapper.Map<ActorDto>(created);
        }

        public async Task UpdateActorAsync(UpdateActorDto updateActorDto)
        {
            _logger.LogInformation("Updating actor with ID: {ActorId}", updateActorDto.Id);

            // Check if actor exists
            var existing = await _actorRepository.GetByIdAsync(updateActorDto.Id);
            if (existing == null)
            {
                throw new NotFoundException("Actor", updateActorDto.Id);
            }

            // Map DTO properties to existing tracked entity
            _mapper.Map(updateActorDto, existing);

            // Business validation
            if (string.IsNullOrWhiteSpace(existing.Name))
            {
                throw new BadRequestException("Actor name is required");
            }

            // Set update time (CreatedAt already preserved in existing entity)
            existing.UpdatedAt = DateTime.UtcNow;

            // Update in repository
            await _actorRepository.UpdateAsync(existing);
            _logger.LogInformation("Actor updated successfully: {ActorId}", existing.Id);
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
            actor.DeletedAt = DateTime.UtcNow;
            // TODO: Set DeletedBy from current user context when authentication is available
            // entity.DeletedBy = currentUserId;
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
