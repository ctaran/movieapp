using MovieApp.Core.Models;

namespace MovieApp.Infrastructure.Services
{
    public static class MovieMapper
    {
        public static Movie ToMovie(this TMDBMovieDTO dto)
        {
            return new Movie
            {
                Id = dto.Id,
                Title = dto.Title,
                Overview = dto.Overview,
                PosterPath = dto.PosterPath,
                BackdropPath = dto.BackdropPath,
                ReleaseDate = dto.ReleaseDate,
                VoteAverage = dto.VoteAverage,
                VoteCount = dto.VoteCount,
                Popularity = dto.Popularity,
                GenreIds = dto.GenreIds,
                Genres = new List<string>(), // Will be populated by the service
                OriginalLanguage = dto.OriginalLanguage,
                OriginalTitle = dto.OriginalTitle,
                Adult = dto.Adult,
                Video = dto.Video
            };
        }
    }
} 