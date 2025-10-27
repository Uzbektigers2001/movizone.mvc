using System;
using System.Collections.Generic;
using System.Linq;
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
    /// Application service for TV series business logic
    /// Uses DTOs and AutoMapper for clean separation from database models
    /// </summary>
    public class TVSeriesApplicationService : ITVSeriesApplicationService
    {
        private readonly ITVSeriesRepository _seriesRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<TVSeriesApplicationService> _logger;

        public TVSeriesApplicationService(
            ITVSeriesRepository seriesRepository,
            IMapper mapper,
            ILogger<TVSeriesApplicationService> logger)
        {
            _seriesRepository = seriesRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<TVSeriesDto>> GetAllSeriesAsync()
        {
            try
            {
                _logger.LogInformation("Fetching all TV series");
                var series = await _seriesRepository.GetAllAsync();
                return _mapper.Map<IEnumerable<TVSeriesDto>>(series);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching all TV series");
                throw;
            }
        }

        public async Task<IEnumerable<TVSeriesDto>> GetFeaturedSeriesAsync()
        {
            try
            {
                _logger.LogInformation("Fetching featured TV series");
                var series = await _seriesRepository.GetFeaturedSeriesAsync();
                return _mapper.Map<IEnumerable<TVSeriesDto>>(series);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching featured TV series");
                throw;
            }
        }

        public async Task<IEnumerable<TVSeriesDto>> SearchSeriesAsync(string? searchTerm, string? genre)
        {
            try
            {
                _logger.LogInformation("Searching TV series with term: {SearchTerm}, genre: {Genre}", searchTerm, genre);
                var series = await _seriesRepository.SearchSeriesAsync(searchTerm, genre);
                return _mapper.Map<IEnumerable<TVSeriesDto>>(series);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching TV series with term: {SearchTerm}, genre: {Genre}", searchTerm, genre);
                throw;
            }
        }

        public async Task<TVSeriesDto?> GetSeriesByIdAsync(int id)
        {
            try
            {
                _logger.LogInformation("Fetching TV series with ID: {SeriesId}", id);
                var series = await _seriesRepository.GetByIdAsync(id);

                if (series == null)
                {
                    _logger.LogWarning("TV series with ID {SeriesId} not found", id);
                    return null;
                }

                return _mapper.Map<TVSeriesDto>(series);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching TV series with ID: {SeriesId}", id);
                throw;
            }
        }

        public async Task<TVSeriesDto> CreateSeriesAsync(CreateTVSeriesDto createSeriesDto)
        {
            try
            {
                _logger.LogInformation("Creating new TV series: {SeriesTitle}", createSeriesDto.Title);

                // Map DTO to Model
                var series = _mapper.Map<TVSeries>(createSeriesDto);

                // Business validation (additional to DTO validation)
                if (string.IsNullOrWhiteSpace(series.Title))
                {
                    throw new BadRequestException("TV series title is required");
                }

                if (series.Seasons < 1 || series.Seasons > 100)
                {
                    throw new BadRequestException("Seasons must be between 1 and 100");
                }

                if (series.TotalEpisodes < 1)
                {
                    throw new BadRequestException("Total episodes must be at least 1");
                }

                // Set timestamps
                series.CreatedAt = DateTime.UtcNow;

                // Save to repository
                var created = await _seriesRepository.AddAsync(series);

                _logger.LogInformation("TV series created successfully with ID: {SeriesId}", created.Id);

                // Map back to DTO and return
                return _mapper.Map<TVSeriesDto>(created);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating TV series: {SeriesTitle}", createSeriesDto.Title);
                throw;
            }
        }

        public async Task UpdateSeriesAsync(UpdateTVSeriesDto updateSeriesDto)
        {
            try
            {
                _logger.LogInformation("Updating TV series with ID: {SeriesId}", updateSeriesDto.Id);

                // Check if series exists
                var existing = await _seriesRepository.GetByIdAsync(updateSeriesDto.Id);
                if (existing == null)
                {
                    throw new NotFoundException("TV Series", updateSeriesDto.Id);
                }

                // Map DTO properties to existing tracked entity
                _mapper.Map(updateSeriesDto, existing);

                // Business validation
                if (string.IsNullOrWhiteSpace(existing.Title))
                {
                    throw new BadRequestException("TV series title is required");
                }

                // Set update time (CreatedAt already preserved in existing entity)
                existing.UpdatedAt = DateTime.UtcNow;

                // Update in repository
                await _seriesRepository.UpdateAsync(existing);
                _logger.LogInformation("TV series updated successfully: {SeriesId}", existing.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating TV series with ID: {SeriesId}", updateSeriesDto.Id);
                throw;
            }
        }

        public async Task DeleteSeriesAsync(int id)
        {
            try
            {
                _logger.LogInformation("Deleting TV series with ID: {SeriesId}", id);

                var series = await _seriesRepository.GetByIdAsync(id);
                if (series == null)
                {
                    throw new NotFoundException("TV Series", id);
                }

                // Soft delete
                series.IsDeleted = true;
                series.DeletedAt = DateTime.UtcNow;
                // TODO: Set DeletedBy from current user context when authentication is available
                // entity.DeletedBy = currentUserId;
                series.UpdatedAt = DateTime.UtcNow;
                await _seriesRepository.UpdateAsync(series);

                _logger.LogInformation("TV series soft-deleted successfully: {SeriesId}", id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting TV series with ID: {SeriesId}", id);
                throw;
            }
        }

        public async Task<IEnumerable<string>> GetAllGenresAsync()
        {
            try
            {
                _logger.LogInformation("Fetching all TV series genres");
                // Use database-level distinct query instead of fetching all series
                return await _seriesRepository.GetDistinctGenresAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching all TV series genres");
                throw;
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            try
            {
                return await _seriesRepository.ExistsAsync(s => s.Id == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if TV series exists with ID: {SeriesId}", id);
                throw;
            }
        }
    }
}
