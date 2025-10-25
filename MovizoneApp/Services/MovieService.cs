using MovizoneApp.Models;

namespace MovizoneApp.Services
{
    public class MovieService : IMovieService
    {
        private readonly List<Movie> _movies;

        public MovieService()
        {
            _movies = new List<Movie>
            {
                new Movie
                {
                    Id = 1,
                    Title = "The Edge of Tomorrow",
                    Description = "A soldier fighting aliens gets to relive the same day over and over again, the day restarting every time he dies.",
                    Year = 2014,
                    Rating = 8.4,
                    Genre = "Action, Sci-Fi",
                    Duration = 113,
                    Country = "USA",
                    Director = "Doug Liman",
                    CoverImage = "/img/covers/cover12.jpg",
                    PosterImage = "/img/posters/poster12.jpg",
                    VideoUrl = "#",
                    Actors = new List<string> { "Tom Cruise", "Emily Blunt" },
                    IsFeatured = true,
                    ReleaseDate = new DateTime(2014, 6, 6)
                },
                new Movie
                {
                    Id = 2,
                    Title = "Interstellar",
                    Description = "A team of explorers travel through a wormhole in space in an attempt to ensure humanity's survival.",
                    Year = 2014,
                    Rating = 8.7,
                    Genre = "Adventure, Drama, Sci-Fi",
                    Duration = 169,
                    Country = "USA",
                    Director = "Christopher Nolan",
                    CoverImage = "/img/covers/cover1.jpg",
                    PosterImage = "/img/posters/poster1.jpg",
                    VideoUrl = "#",
                    Actors = new List<string> { "Matthew McConaughey", "Anne Hathaway" },
                    IsFeatured = true,
                    ReleaseDate = new DateTime(2014, 11, 7)
                },
                new Movie
                {
                    Id = 3,
                    Title = "Inception",
                    Description = "A thief who steals corporate secrets through the use of dream-sharing technology is given the inverse task of planting an idea into the mind of a C.E.O.",
                    Year = 2010,
                    Rating = 8.8,
                    Genre = "Action, Sci-Fi, Thriller",
                    Duration = 148,
                    Country = "USA",
                    Director = "Christopher Nolan",
                    CoverImage = "/img/covers/cover2.jpg",
                    VideoUrl = "#",
                    Actors = new List<string> { "Leonardo DiCaprio", "Joseph Gordon-Levitt" },
                    IsFeatured = true,
                    ReleaseDate = new DateTime(2010, 7, 16)
                },
                new Movie
                {
                    Id = 4,
                    Title = "The Dark Knight",
                    Description = "When the menace known as the Joker wreaks havoc and chaos on the people of Gotham, Batman must accept one of the greatest psychological and physical tests of his ability to fight injustice.",
                    Year = 2008,
                    Rating = 9.0,
                    Genre = "Action, Crime, Drama",
                    Duration = 152,
                    Country = "USA",
                    Director = "Christopher Nolan",
                    CoverImage = "/img/covers/cover3.jpg",
                    VideoUrl = "#",
                    Actors = new List<string> { "Christian Bale", "Heath Ledger" },
                    IsFeatured = true,
                    ReleaseDate = new DateTime(2008, 7, 18)
                },
                new Movie
                {
                    Id = 5,
                    Title = "The Matrix",
                    Description = "A computer hacker learns from mysterious rebels about the true nature of his reality and his role in the war against its controllers.",
                    Year = 1999,
                    Rating = 8.7,
                    Genre = "Action, Sci-Fi",
                    Duration = 136,
                    Country = "USA",
                    Director = "Lana Wachowski, Lilly Wachowski",
                    CoverImage = "/img/covers/cover4.jpg",
                    VideoUrl = "#",
                    Actors = new List<string> { "Keanu Reeves", "Laurence Fishburne" },
                    IsFeatured = false,
                    ReleaseDate = new DateTime(1999, 3, 31)
                },
                new Movie
                {
                    Id = 6,
                    Title = "Gladiator",
                    Description = "A former Roman General sets out to exact vengeance against the corrupt emperor who murdered his family and sent him into slavery.",
                    Year = 2000,
                    Rating = 8.5,
                    Genre = "Action, Drama",
                    Duration = 155,
                    Country = "USA",
                    Director = "Ridley Scott",
                    CoverImage = "/img/covers/cover5.jpg",
                    VideoUrl = "#",
                    Actors = new List<string> { "Russell Crowe", "Joaquin Phoenix" },
                    IsFeatured = false,
                    ReleaseDate = new DateTime(2000, 5, 5)
                },
                new Movie
                {
                    Id = 7,
                    Title = "The Shawshank Redemption",
                    Description = "Two imprisoned men bond over a number of years, finding solace and eventual redemption through acts of common decency.",
                    Year = 1994,
                    Rating = 9.3,
                    Genre = "Drama",
                    Duration = 142,
                    Country = "USA",
                    Director = "Frank Darabont",
                    CoverImage = "/img/covers/cover6.jpg",
                    VideoUrl = "#",
                    Actors = new List<string> { "Tim Robbins", "Morgan Freeman" },
                    IsFeatured = false,
                    ReleaseDate = new DateTime(1994, 9, 23)
                },
                new Movie
                {
                    Id = 8,
                    Title = "Pulp Fiction",
                    Description = "The lives of two mob hitmen, a boxer, a gangster and his wife intertwine in four tales of violence and redemption.",
                    Year = 1994,
                    Rating = 8.9,
                    Genre = "Crime, Drama",
                    Duration = 154,
                    Country = "USA",
                    Director = "Quentin Tarantino",
                    CoverImage = "/img/covers/cover7.jpg",
                    VideoUrl = "#",
                    Actors = new List<string> { "John Travolta", "Uma Thurman" },
                    IsFeatured = false,
                    ReleaseDate = new DateTime(1994, 10, 14)
                }
            };
        }

        public List<Movie> GetAllMovies() => _movies;

        public Movie? GetMovieById(int id) => _movies.FirstOrDefault(m => m.Id == id);

        public List<Movie> GetFeaturedMovies() => _movies.Where(m => m.IsFeatured).ToList();

        public List<Movie> SearchMovies(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return _movies;

            query = query.ToLower();
            return _movies.Where(m =>
                m.Title.ToLower().Contains(query) ||
                m.Description.ToLower().Contains(query) ||
                m.Genre.ToLower().Contains(query)
            ).ToList();
        }

        public void AddMovie(Movie movie)
        {
            movie.Id = _movies.Any() ? _movies.Max(m => m.Id) + 1 : 1;
            _movies.Add(movie);
        }

        public void UpdateMovie(Movie movie)
        {
            var existingMovie = GetMovieById(movie.Id);
            if (existingMovie != null)
            {
                existingMovie.Title = movie.Title;
                existingMovie.Description = movie.Description;
                existingMovie.Year = movie.Year;
                existingMovie.Rating = movie.Rating;
                existingMovie.Genre = movie.Genre;
                existingMovie.Duration = movie.Duration;
                existingMovie.Country = movie.Country;
                existingMovie.Director = movie.Director;
                existingMovie.CoverImage = movie.CoverImage;
                existingMovie.VideoUrl = movie.VideoUrl;
                existingMovie.Actors = movie.Actors;
                existingMovie.IsFeatured = movie.IsFeatured;
                existingMovie.ReleaseDate = movie.ReleaseDate;
            }
        }

        public void DeleteMovie(int id)
        {
            var movie = GetMovieById(id);
            if (movie != null)
            {
                _movies.Remove(movie);
            }
        }
    }
}
