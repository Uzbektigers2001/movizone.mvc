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
    /// Application service for movie business logic
    /// </summary>
    public class MovieApplicationService : IMovieApplicationService
    {
        private readonly IMovieRepository _movieRepository;
        private readonly ILogger<MovieApplicationService> _logger;

        public MovieApplicationService(
            IMovieRepository movieRepository,
            ILogger<MovieApplicationService> logger)
        {
            _movieRepository = movieRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<Movie>> GetAllMoviesAsync()
        {
            _logger.LogInformation("Fetching all movies");
            return await _movieRepository.GetAllAsync();
        }

        public async Task<IEnumerable<Movie>> GetFeaturedMoviesAsync()
        {
            _logger.LogInformation("Fetching featured movies");
            return await _movieRepository.GetFeaturedMoviesAsync();
        }

        public async Task<IEnumerable<Movie>> SearchMoviesAsync(string? searchTerm, string? genre)
        {
            _logger.LogInformation("Searching movies with term: {SearchTerm}, genre: {Genre}", searchTerm, genre);
            return await _movieRepository.SearchMoviesAsync(searchTerm, genre);
        }

        public async Task<Movie?> GetMovieByIdAsync(int id)
        {
            _logger.LogInformation("Fetching movie with ID: {MovieId}", id);
            var movie = await _movieRepository.GetByIdAsync(id);

            if (movie == null)
            {
                _logger.LogWarning("Movie with ID {MovieId} not found", id);
            }

            return movie;
        }

        public async Task<Movie> CreateMovieAsync(Movie movie)
        {
            _logger.LogInformation("Creating new movie: {MovieTitle}", movie.Title);

            // Business validation
            if (string.IsNullOrWhiteSpace(movie.Title))
            {
                throw new BadRequestException("Movie title is required");
            }

            if (movie.Year < 1900 || movie.Year > DateTime.UtcNow.Year + 5)
            {
                throw new BadRequestException($"Invalid year: {movie.Year}");
            }

            if (movie.Rating < 0 || movie.Rating > 10)
            {
                throw new BadRequestException("Rating must be between 0 and 10");
            }

            movie.CreatedAt = DateTime.UtcNow;
            var created = await _movieRepository.AddAsync(movie);

            _logger.LogInformation("Movie created successfully with ID: {MovieId}", created.Id);
            return created;
        }

        public async Task UpdateMovieAsync(Movie movie)
        {
            _logger.LogInformation("Updating movie with ID: {MovieId}", movie.Id);

            var existing = await _movieRepository.GetByIdAsync(movie.Id);
            if (existing == null)
            {
                throw new NotFoundException("Movie", movie.Id);
            }

            // Business validation
            if (string.IsNullOrWhiteSpace(movie.Title))
            {
                throw new BadRequestException("Movie title is required");
            }

            movie.UpdatedAt = DateTime.UtcNow;
            movie.CreatedAt = existing.CreatedAt; // Preserve creation date

            await _movieRepository.UpdateAsync(movie);
            _logger.LogInformation("Movie updated successfully: {MovieId}", movie.Id);
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
