using MovizoneApp.Models;

namespace MovizoneApp.Services
{
    public interface IMovieService
    {
        List<Movie> GetAllMovies();
        Movie? GetMovieById(int id);
        List<Movie> GetFeaturedMovies();
        List<Movie> SearchMovies(string query);
        void AddMovie(Movie movie);
        void UpdateMovie(Movie movie);
        void DeleteMovie(int id);
    }
}
