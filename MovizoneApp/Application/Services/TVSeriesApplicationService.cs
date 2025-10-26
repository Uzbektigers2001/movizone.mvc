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
            _logger.LogInformation("Fetching all TV series");
            var series = await _seriesRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<TVSeriesDto>>(series);
        }

        public async Task<IEnumerable<TVSeriesDto>> GetFeaturedSeriesAsync()
        {
            _logger.LogInformation("Fetching featured TV series");
            var series = await _seriesRepository.GetFeaturedSeriesAsync();
            return _mapper.Map<IEnumerable<TVSeriesDto>>(series);
        }

        public async Task<IEnumerable<TVSeriesDto>> SearchSeriesAsync(string? searchTerm, string? genre)
        {
            _logger.LogInformation("Searching TV series with term: {SearchTerm}, genre: {Genre}", searchTerm, genre);
            var series = await _seriesRepository.SearchSeriesAsync(searchTerm, genre);
            return _mapper.Map<IEnumerable<TVSeriesDto>>(series);
        }

        public async Task<TVSeriesDto?> GetSeriesByIdAsync(int id)
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

        public async Task<TVSeriesDto> CreateSeriesAsync(CreateTVSeriesDto createSeriesDto)
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

        public async Task UpdateSeriesAsync(UpdateTVSeriesDto updateSeriesDto)
        {
            _logger.LogInformation("Updating TV series with ID: {SeriesId}", updateSeriesDto.Id);

            // Check if series exists
            var existing = await _seriesRepository.GetByIdAsync(updateSeriesDto.Id);
            if (existing == null)
            {
                throw new NotFoundException("TV Series", updateSeriesDto.Id);
            }

            // Map DTO to Model
            var series = _mapper.Map<TVSeries>(updateSeriesDto);

            // Business validation
            if (string.IsNullOrWhiteSpace(series.Title))
            {
                throw new BadRequestException("TV series title is required");
            }

            // Preserve creation date and set update time
            series.CreatedAt = existing.CreatedAt;
            series.UpdatedAt = DateTime.UtcNow;

            // Update in repository
            await _seriesRepository.UpdateAsync(series);
            _logger.LogInformation("TV series updated successfully: {SeriesId}", series.Id);
        }

        public async Task DeleteSeriesAsync(int id)
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

        public async Task<IEnumerable<string>> GetAllGenresAsync()
        {
            _logger.LogInformation("Fetching all TV series genres");
            var series = await _seriesRepository.GetAllAsync();
            return series.Select(s => s.Genre).Distinct().OrderBy(g => g).ToList();
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _seriesRepository.ExistsAsync(s => s.Id == id);
        }
    }
}
