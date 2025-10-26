using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MovizoneApp.Application.Interfaces;
using MovizoneApp.Core.Exceptions;
using MovizoneApp.Core.Interfaces;
using MovizoneApp.Models;

namespace MovizoneApp.Application.Services
{
    /// <summary>
    /// Application service for TV series business logic
    /// </summary>
    public class TVSeriesApplicationService : ITVSeriesApplicationService
    {
        private readonly ITVSeriesRepository _seriesRepository;
        private readonly ILogger<TVSeriesApplicationService> _logger;

        public TVSeriesApplicationService(
            ITVSeriesRepository seriesRepository,
            ILogger<TVSeriesApplicationService> logger)
        {
            _seriesRepository = seriesRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<TVSeries>> GetAllSeriesAsync()
        {
            _logger.LogInformation("Fetching all TV series");
            return await _seriesRepository.GetAllAsync();
        }

        public async Task<IEnumerable<TVSeries>> GetFeaturedSeriesAsync()
        {
            _logger.LogInformation("Fetching featured TV series");
            return await _seriesRepository.GetFeaturedSeriesAsync();
        }

        public async Task<IEnumerable<TVSeries>> SearchSeriesAsync(string? searchTerm, string? genre)
        {
            _logger.LogInformation("Searching TV series with term: {SearchTerm}, genre: {Genre}", searchTerm, genre);
            return await _seriesRepository.SearchSeriesAsync(searchTerm, genre);
        }

        public async Task<TVSeries?> GetSeriesByIdAsync(int id)
        {
            _logger.LogInformation("Fetching TV series with ID: {SeriesId}", id);
            var series = await _seriesRepository.GetByIdAsync(id);

            if (series == null)
            {
                _logger.LogWarning("TV series with ID {SeriesId} not found", id);
            }

            return series;
        }

        public async Task<TVSeries> CreateSeriesAsync(TVSeries series)
        {
            _logger.LogInformation("Creating new TV series: {SeriesTitle}", series.Title);

            // Business validation
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

            series.CreatedAt = DateTime.UtcNow;
            var created = await _seriesRepository.AddAsync(series);

            _logger.LogInformation("TV series created successfully with ID: {SeriesId}", created.Id);
            return created;
        }

        public async Task UpdateSeriesAsync(TVSeries series)
        {
            _logger.LogInformation("Updating TV series with ID: {SeriesId}", series.Id);

            var existing = await _seriesRepository.GetByIdAsync(series.Id);
            if (existing == null)
            {
                throw new NotFoundException("TV Series", series.Id);
            }

            // Business validation
            if (string.IsNullOrWhiteSpace(series.Title))
            {
                throw new BadRequestException("TV series title is required");
            }

            series.UpdatedAt = DateTime.UtcNow;
            series.CreatedAt = existing.CreatedAt;

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
