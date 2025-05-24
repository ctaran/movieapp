using System.Collections.Generic;
using System.Threading.Tasks;
using MovieApp.Core.Models;

namespace MovieApp.Core.Interfaces
{
    public interface IGenreService
    {
        Task<int?> GetGenreId(string genreName);
        Task<string?> GetGenreName(int genreId);
        Task<IEnumerable<Genre>> GetAllGenres();
    }
} 