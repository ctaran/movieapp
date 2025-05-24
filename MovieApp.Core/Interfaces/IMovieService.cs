using System.Collections.Generic;
using System.Threading.Tasks;
using MovieApp.Core.Models;

namespace MovieApp.Core.Interfaces
{
    public interface IMovieService
    {
        Task<IEnumerable<Movie>> GetLatestMoviesAsync();
        Task<IEnumerable<Movie>> GetTopRatedMoviesAsync();
        Task<IEnumerable<Movie>> SearchMoviesAsync(string query, string? genre = null);
        Task<Movie> GetMovieDetailsAsync(int movieId);
    }
} 