using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MovieApp.Core.Interfaces;
using MovieApp.Core.Models;

namespace MovieApp.Infrastructure.Services
{
    public class GenreService : IGenreService
    {
        private readonly Dictionary<string, int> _genreMap = new()
        {
            { "action", 28 },
            { "adventure", 12 },
            { "animation", 16 },
            { "comedy", 35 },
            { "crime", 80 },
            { "documentary", 99 },
            { "drama", 18 },
            { "family", 10751 },
            { "fantasy", 14 },
            { "history", 36 },
            { "horror", 27 },
            { "music", 10402 },
            { "mystery", 9648 },
            { "romance", 10749 },
            { "science fiction", 878 },
            { "thriller", 53 },
            { "war", 10752 },
            { "western", 37 }
        };

        public Task<int?> GetGenreId(string genreName)
        {
            if (string.IsNullOrWhiteSpace(genreName))
                return Task.FromResult<int?>(null);

            var normalizedGenre = genreName.ToLower().Trim();
            return Task.FromResult<int?>(_genreMap.TryGetValue(normalizedGenre, out var id) ? id : null);
        }

        public Task<string?> GetGenreName(int genreId)
        {
            var genre = _genreMap.FirstOrDefault(x => x.Value == genreId);
            return Task.FromResult<string?>(genre.Key);
        }

        public Task<IEnumerable<Genre>> GetAllGenres()
        {
            var genres = _genreMap.Select(x => new Genre { Id = x.Value, Name = x.Key });
            return Task.FromResult(genres);
        }
    }
} 