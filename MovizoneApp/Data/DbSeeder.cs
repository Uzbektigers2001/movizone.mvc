using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MovizoneApp.Models;

namespace MovizoneApp.Data
{
    /// <summary>
    /// Database seeder for initial data
    /// </summary>
    public class DbSeeder
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger _logger;

        public DbSeeder(ApplicationDbContext context, ILogger logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task SeedAsync()
        {
            try
            {
                // Ensure database is created (works with both InMemory and Relational databases)
                await _context.Database.EnsureCreatedAsync();

                // Seed Users
                if (!await _context.Users.AnyAsync())
                {
                    _logger.LogInformation("Seeding users...");
                    var users = GetSeedUsers();
                    await _context.Users.AddRangeAsync(users);
                    await _context.SaveChangesAsync();
                }

                // Seed Movies
                if (!await _context.Movies.AnyAsync())
                {
                    _logger.LogInformation("Seeding movies...");
                    var movies = GetSeedMovies();
                    await _context.Movies.AddRangeAsync(movies);
                    await _context.SaveChangesAsync();
                }

                // Seed TV Series
                if (!await _context.TVSeries.AnyAsync())
                {
                    _logger.LogInformation("Seeding TV series...");
                    var series = GetSeedTVSeries();
                    await _context.TVSeries.AddRangeAsync(series);
                    await _context.SaveChangesAsync();
                }

                // Seed Actors
                if (!await _context.Actors.AnyAsync())
                {
                    _logger.LogInformation("Seeding actors...");
                    var actors = GetSeedActors();
                    await _context.Actors.AddRangeAsync(actors);
                    await _context.SaveChangesAsync();
                }

                // Seed MovieActors relationships
                if (!await _context.MovieActors.AnyAsync())
                {
                    _logger.LogInformation("Seeding movie-actor relationships...");
                    var movieActors = GetSeedMovieActors();
                    await _context.MovieActors.AddRangeAsync(movieActors);
                    await _context.SaveChangesAsync();
                }

                // Seed TVSeriesActors relationships
                if (!await _context.TVSeriesActors.AnyAsync())
                {
                    _logger.LogInformation("Seeding series-actor relationships...");
                    var seriesActors = GetSeedTVSeriesActors();
                    await _context.TVSeriesActors.AddRangeAsync(seriesActors);
                    await _context.SaveChangesAsync();
                }

                // Seed Pricing Plans
                if (!await _context.PricingPlans.AnyAsync())
                {
                    _logger.LogInformation("Seeding pricing plans...");
                    var plans = GetSeedPricingPlans();
                    await _context.PricingPlans.AddRangeAsync(plans);
                    await _context.SaveChangesAsync();
                }

                _logger.LogInformation("Database seeding completed successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while seeding the database");
                throw;
            }
        }

        private List<User> GetSeedUsers()
        {
            var adminUser = new User
            {
                Name = "Admin",
                Email = "admin@hotflix.com",
                Role = "Admin",
                IsActive = true,
                Avatar = "/img/user.svg"
            };
            adminUser.SetPassword("admin123");

            var regularUser = new User
            {
                Name = "John Doe",
                Email = "john@example.com",
                Role = "User",
                IsActive = true,
                Avatar = "/img/user.svg"
            };
            regularUser.SetPassword("password123");

            return new List<User> { adminUser, regularUser };
        }

        private List<Movie> GetSeedMovies()
        {
            return new List<Movie>
            {
                new Movie
                {
                    Title = "The Shawshank Redemption",
                    Description = "Two imprisoned men bond over a number of years, finding solace and eventual redemption through acts of common decency.",
                    Year = 1994,
                    Rating = 9.3,
                    Genre = "Drama",
                    Duration = 142,
                    Country = "USA",
                    Director = "Frank Darabont",
                    CoverImage = "/img/covers/cover1.jpg",
                    VideoUrl = "https://example.com/video1.mp4",
                    IsFeatured = true,
                    ReleaseDate = DateTime.SpecifyKind(new DateTime(1994, 9, 23), DateTimeKind.Utc)
                },
                new Movie
                {
                    Title = "The Godfather",
                    Description = "The aging patriarch of an organized crime dynasty transfers control of his clandestine empire to his reluctant son.",
                    Year = 1972,
                    Rating = 9.2,
                    Genre = "Crime",
                    Duration = 175,
                    Country = "USA",
                    Director = "Francis Ford Coppola",
                    CoverImage = "/img/covers/cover2.jpg",
                    VideoUrl = "https://example.com/video2.mp4",
                    IsFeatured = true,
                    ReleaseDate = DateTime.SpecifyKind(new DateTime(1972, 3, 24), DateTimeKind.Utc)
                },
                new Movie
                {
                    Title = "The Dark Knight",
                    Description = "When the menace known as the Joker wreaks havoc and chaos on the people of Gotham, Batman must accept one of the greatest psychological and physical tests.",
                    Year = 2008,
                    Rating = 9.0,
                    Genre = "Action",
                    Duration = 152,
                    Country = "USA",
                    Director = "Christopher Nolan",
                    CoverImage = "/img/covers/cover3.jpg",
                    VideoUrl = "https://example.com/video3.mp4",
                    IsFeatured = true,
                    ReleaseDate = DateTime.SpecifyKind(new DateTime(2008, 7, 18), DateTimeKind.Utc)
                },
                new Movie
                {
                    Title = "Inception",
                    Description = "A thief who steals corporate secrets through the use of dream-sharing technology is given the inverse task of planting an idea.",
                    Year = 2010,
                    Rating = 8.8,
                    Genre = "Sci-Fi",
                    Duration = 148,
                    Country = "USA",
                    Director = "Christopher Nolan",
                    CoverImage = "/img/covers/cover4.jpg",
                    VideoUrl = "https://example.com/video4.mp4",
                    IsFeatured = false,
                    ReleaseDate = DateTime.SpecifyKind(new DateTime(2010, 7, 16), DateTimeKind.Utc)
                }
            };
        }

        private List<TVSeries> GetSeedTVSeries()
        {
            return new List<TVSeries>
            {
                new TVSeries
                {
                    Title = "Breaking Bad",
                    Description = "A high school chemistry teacher turned meth cook partners with a former student to secure his family's future.",
                    Year = 2008,
                    Rating = 9.5,
                    Genre = "Crime",
                    Seasons = 5,
                    TotalEpisodes = 62,
                    Country = "USA",
                    Creator = "Vince Gilligan",
                    CoverImage = "/img/covers/cover5.jpg",
                    IsFeatured = true,
                    FirstAired = DateTime.SpecifyKind(new DateTime(2008, 1, 20), DateTimeKind.Utc),
                    Status = "Completed"
                },
                new TVSeries
                {
                    Title = "Game of Thrones",
                    Description = "Nine noble families fight for control over the lands of Westeros, while an ancient enemy returns.",
                    Year = 2011,
                    Rating = 9.3,
                    Genre = "Fantasy",
                    Seasons = 8,
                    TotalEpisodes = 73,
                    Country = "USA",
                    Creator = "David Benioff, D.B. Weiss",
                    CoverImage = "/img/covers/cover6.jpg",
                    IsFeatured = true,
                    FirstAired = DateTime.SpecifyKind(new DateTime(2011, 4, 17), DateTimeKind.Utc),
                    Status = "Completed"
                }
            };
        }

        private List<Actor> GetSeedActors()
        {
            return new List<Actor>
            {
                new Actor
                {
                    Name = "Leonardo DiCaprio",
                    Bio = "American actor and film producer known for his work in biographical and period films.",
                    BirthDate = DateTime.SpecifyKind(new DateTime(1974, 11, 11), DateTimeKind.Utc),
                    Country = "USA",
                    Photo = "/img/covers/actor.jpg"
                },
                new Actor
                {
                    Name = "Bryan Cranston",
                    Bio = "American actor, director, and producer best known for portraying Walter White in Breaking Bad.",
                    BirthDate = DateTime.SpecifyKind(new DateTime(1956, 3, 7), DateTimeKind.Utc),
                    Country = "USA",
                    Photo = "/img/covers/actor.jpg"
                },
                new Actor
                {
                    Name = "Christian Bale",
                    Bio = "English actor known for his versatility and intensive method acting.",
                    BirthDate = DateTime.SpecifyKind(new DateTime(1974, 1, 30), DateTimeKind.Utc),
                    Country = "UK",
                    Photo = "/img/covers/actor.jpg"
                },
                new Actor
                {
                    Name = "Tim Robbins",
                    Bio = "American actor, screenwriter, director, and producer.",
                    BirthDate = DateTime.SpecifyKind(new DateTime(1958, 10, 16), DateTimeKind.Utc),
                    Country = "USA",
                    Photo = "/img/covers/actor.jpg"
                },
                new Actor
                {
                    Name = "Morgan Freeman",
                    Bio = "American actor and film narrator known for his distinctive deep voice.",
                    BirthDate = DateTime.SpecifyKind(new DateTime(1937, 6, 1), DateTimeKind.Utc),
                    Country = "USA",
                    Photo = "/img/covers/actor.jpg"
                },
                new Actor
                {
                    Name = "Aaron Paul",
                    Bio = "American actor best known for his role as Jesse Pinkman in Breaking Bad.",
                    BirthDate = DateTime.SpecifyKind(new DateTime(1979, 8, 27), DateTimeKind.Utc),
                    Country = "USA",
                    Photo = "/img/covers/actor.jpg"
                }
            };
        }

        private List<PricingPlan> GetSeedPricingPlans()
        {
            return new List<PricingPlan>
            {
                new PricingPlan
                {
                    Name = "Basic",
                    Price = 9.99m,
                    Period = "Monthly",
                    Features = new List<string> { "HD Quality", "Watch on 1 device", "Limited content" },
                    IsPopular = false
                },
                new PricingPlan
                {
                    Name = "Standard",
                    Price = 14.99m,
                    Period = "Monthly",
                    Features = new List<string> { "Full HD Quality", "Watch on 2 devices", "Full content library", "Download content" },
                    IsPopular = true
                },
                new PricingPlan
                {
                    Name = "Premium",
                    Price = 19.99m,
                    Period = "Monthly",
                    Features = new List<string> { "4K Ultra HD", "Watch on 4 devices", "Full content library", "Download content", "Priority support" },
                    IsPopular = false
                }
            };
        }

        private List<MovieActor> GetSeedMovieActors()
        {
            // Note: These use hardcoded IDs which will match seeded data
            // In InMemory database, IDs start from 1
            return new List<MovieActor>
            {
                // The Shawshank Redemption (MovieId = 1)
                new MovieActor { MovieId = 1, ActorId = 4, Role = "Andy Dufresne", DisplayOrder = 1 }, // Tim Robbins
                new MovieActor { MovieId = 1, ActorId = 5, Role = "Ellis Boyd 'Red' Redding", DisplayOrder = 2 }, // Morgan Freeman

                // Inception (MovieId = 4)
                new MovieActor { MovieId = 4, ActorId = 1, Role = "Dom Cobb", DisplayOrder = 1 }, // Leonardo DiCaprio

                // The Dark Knight (MovieId = 3)
                new MovieActor { MovieId = 3, ActorId = 3, Role = "Bruce Wayne / Batman", DisplayOrder = 1 } // Christian Bale
            };
        }

        private List<TVSeriesActor> GetSeedTVSeriesActors()
        {
            // Note: These use hardcoded IDs which will match seeded data
            // In InMemory database, IDs start from 1
            return new List<TVSeriesActor>
            {
                // Breaking Bad (TVSeriesId = 1)
                new TVSeriesActor { TVSeriesId = 1, ActorId = 2, Role = "Walter White", DisplayOrder = 1 }, // Bryan Cranston
                new TVSeriesActor { TVSeriesId = 1, ActorId = 6, Role = "Jesse Pinkman", DisplayOrder = 2 }  // Aaron Paul
            };
        }
    }
}
