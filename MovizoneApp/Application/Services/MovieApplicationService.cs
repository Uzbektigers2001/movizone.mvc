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
    /// Application service for movie business logic
    /// Uses DTOs and AutoMapper for clean separation from database models
    /// </summary>
    public class MovieApplicationService : IMovieApplicationService
    {
        private readonly IMovieRepository _movieRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<MovieApplicationService> _logger;

        public MovieApplicationService(
            IMovieRepository movieRepository,
            IMapper mapper,
            ILogger<MovieApplicationService> logger)
        {
            _movieRepository = movieRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<MovieDto>> GetAllMoviesAsync()
        {
            _logger.LogInformation("Fetching all movies");
            var movies = await _movieRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<MovieDto>>(movies);
        }

        public async Task<IEnumerable<MovieDto>> GetFeaturedMoviesAsync()
        {
            _logger.LogInformation("Fetching featured movies");
            var movies = await _movieRepository.GetFeaturedMoviesAsync();
            return _mapper.Map<IEnumerable<MovieDto>>(movies);
        }

        public async Task<IEnumerable<MovieDto>> SearchMoviesAsync(string? searchTerm, string? genre)
        {
            _logger.LogInformation("Searching movies with term: {SearchTerm}, genre: {Genre}", searchTerm, genre);
            var movies = await _movieRepository.SearchMoviesAsync(searchTerm, genre);
            return _mapper.Map<IEnumerable<MovieDto>>(movies);
        }

        public async Task<MovieDto?> GetMovieByIdAsync(int id)
        {
            _logger.LogInformation("Fetching movie with ID: {MovieId}", id);
            var movie = await _movieRepository.GetByIdAsync(id);

            if (movie == null)
            {
                _logger.LogWarning("Movie with ID {MovieId} not found", id);
                return null;
            }

            return _mapper.Map<MovieDto>(movie);
        }

        public async Task<MovieDto> CreateMovieAsync(CreateMovieDto createMovieDto)
        {
            _logger.LogInformation("Creating new movie: {MovieTitle}", createMovieDto.Title);

            // Map DTO to Model
            var movie = _mapper.Map<Movie>(createMovieDto);

            // Business validation (additional to DTO validation)
            if (movie.Year < 1900 || movie.Year > DateTime.UtcNow.Year + 5)
            {
                throw new BadRequestException($"Invalid year: {movie.Year}");
            }

            if (movie.Rating < 0 || movie.Rating > 10)
            {
                throw new BadRequestException("Rating must be between 0 and 10");
            }

            // Set timestamps
            movie.CreatedAt = DateTime.UtcNow;

            // Save to repository
            var created = await _movieRepository.AddAsync(movie);

            _logger.LogInformation("Movie created successfully with ID: {MovieId}", created.Id);

            // Map back to DTO and return
            return _mapper.Map<MovieDto>(created);
        }

        public async Task UpdateMovieAsync(UpdateMovieDto updateMovieDto)
        {
            _logger.LogInformation("Updating movie with ID: {MovieId}", updateMovieDto.Id);

            // Check if movie exists
            var existing = await _movieRepository.GetByIdAsync(updateMovieDto.Id);
            if (existing == null)
            {
                throw new NotFoundException("Movie", updateMovieDto.Id);
            }

            // Map DTO properties to existing tracked entity
            _mapper.Map(updateMovieDto, existing);

            // Business validation
            if (existing.Year < 1900 || existing.Year > DateTime.UtcNow.Year + 5)
            {
                throw new BadRequestException($"Invalid year: {existing.Year}");
            }

            if (existing.Rating < 0 || existing.Rating > 10)
            {
                throw new BadRequestException("Rating must be between 0 and 10");
            }

            // Set update time (CreatedAt already preserved in existing entity)
            existing.UpdatedAt = DateTime.UtcNow;

            // Update in repository
            await _movieRepository.UpdateAsync(existing);
            _logger.LogInformation("Movie updated successfully: {MovieId}", existing.Id);
        }

        public async Task DeleteMovieAsync(int id)
        {
            _logger.LogInformation("Deleting movie with ID: {MovieId}", id);

            var movie = await _movieRepository.GetByIdAsync(id);
            if (movie == null)
            {
                throw new NotFoundException("Movie", id);
            }

            // Soft delete
            movie.IsDeleted = true;
            movie.DeletedAt = DateTime.UtcNow;
            // TODO: Set DeletedBy from current user context when authentication is available
            // entity.DeletedBy = currentUserId;
            movie.UpdatedAt = DateTime.UtcNow;
            await _movieRepository.UpdateAsync(movie);

            _logger.LogInformation("Movie soft-deleted successfully: {MovieId}", id);
        }

        public async Task<IEnumerable<string>> GetAllGenresAsync()
        {
            _logger.LogInformation("Fetching all movie genres");
            var movies = await _movieRepository.GetAllAsync();
            return movies.Select(m => m.Genre).Distinct().OrderBy(g => g).ToList();
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _movieRepository.ExistsAsync(m => m.Id == id);
        }
    }
}
