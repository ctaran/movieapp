using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MovieApp.Core.Interfaces;
using MovieApp.Core.Models;
using System.Text.Json.Serialization;
using System.Net.Http.Headers;
using System.Linq;

namespace MovieApp.Infrastructure.Services
{
    public class TMDBMovieService : IMovieService
    {
        private readonly HttpClient _httpClient;
        private readonly string _accessToken;
        private readonly string _baseUrl = "https://api.themoviedb.org/3";
        private readonly ILogger<TMDBMovieService> _logger;
        private readonly IGenreService _genreService;

        public TMDBMovieService(
            HttpClient httpClient,
            IConfiguration configuration,
            ILogger<TMDBMovieService> logger,
            IGenreService genreService)
        {
            _httpClient = httpClient;
            _accessToken = configuration["TMDB:AccessToken"] ?? throw new ArgumentNullException(nameof(configuration), "TMDB:AccessToken is not configured");
            _logger = logger;
            _genreService = genreService;
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
        }

        public async Task<IEnumerable<Movie>> GetLatestMoviesAsync()
        {
            try
            {
                _logger.LogInformation("Fetching latest movies from TMDB");
                var response = await _httpClient.GetAsync($"{_baseUrl}/movie/now_playing");
                var content = await response.Content.ReadAsStringAsync();
                _logger.LogInformation("TMDB API Response for latest movies: {Content}", content);

                response.EnsureSuccessStatusCode();
                var result = JsonSerializer.Deserialize<TMDBNowPlayingResponse>(content);
                
                if (result?.Results == null)
                {
                    _logger.LogWarning("No results found in latest movies response");
                    return new List<Movie>();
                }

                _logger.LogInformation("Found {Count} movies in latest movies response", result.Results.Count);
                var movies = new List<Movie>();
                foreach (var dto in result.Results)
                {
                    var movie = dto.ToMovie();
                    _logger.LogInformation("Processing movie: {Title} with genre IDs: {GenreIds}", 
                        movie.Title, string.Join(", ", dto.GenreIds));
                    movie.Genres = await GetGenreNames(dto.GenreIds);
                    movies.Add(movie);
                }

                return movies;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching latest movies from TMDB");
                throw;
            }
        }

        public async Task<IEnumerable<Movie>> GetTopRatedMoviesAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/movie/top_rated");
                var content = await response.Content.ReadAsStringAsync();
                _logger.LogInformation("TMDB API Response for top rated movies: {Content}", content);

                response.EnsureSuccessStatusCode();
                var result = JsonSerializer.Deserialize<TMDBResponse<TMDBMovieDTO>>(content);
                
                if (result?.Results == null)
                {
                    return new List<Movie>();
                }

                var movies = new List<Movie>();
                foreach (var dto in result.Results)
                {
                    var movie = dto.ToMovie();
                    movie.Genres = await GetGenreNames(dto.GenreIds);
                    movies.Add(movie);
                }

                return movies;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching top rated movies from TMDB");
                throw;
            }
        }

        public async Task<IEnumerable<Movie>> SearchMoviesAsync(string? query, string? genre = null)
        {
            try
            {
                // If both query and genre are null/empty, return an empty list (or all movies if desired)
                if (string.IsNullOrWhiteSpace(query) && string.IsNullOrWhiteSpace(genre))
                {
                    return new List<Movie>();
                }

                string url;
                if (!string.IsNullOrWhiteSpace(query))
                {
                    url = $"{_baseUrl}/search/movie?query={Uri.EscapeDataString(query)}";
                    if (!string.IsNullOrEmpty(genre))
                    {
                        var genreId = await _genreService.GetGenreId(genre);
                        if (genreId.HasValue)
                        {
                            url += $"&with_genres={genreId.Value}";
                        }
                    }
                }
                else if (!string.IsNullOrWhiteSpace(genre))
                {
                    // If only genre is provided, use discover endpoint
                    var genreId = await _genreService.GetGenreId(genre);
                    if (genreId.HasValue)
                    {
                        url = $"{_baseUrl}/discover/movie?with_genres={genreId.Value}";
                    }
                    else
                    {
                        return new List<Movie>();
                    }
                }
                else
                {
                    return new List<Movie>();
                }

                var response = await _httpClient.GetAsync(url);
                var content = await response.Content.ReadAsStringAsync();
                _logger.LogInformation("TMDB API Response for search: {Content}", content);

                response.EnsureSuccessStatusCode();
                var result = JsonSerializer.Deserialize<TMDBResponse<TMDBMovieDTO>>(content);
                
                if (result?.Results == null)
                {
                    return new List<Movie>();
                }

                var movies = new List<Movie>();
                foreach (var dto in result.Results)
                {
                    var movie = dto.ToMovie();
                    movie.Genres = await GetGenreNames(dto.GenreIds);
                    movies.Add(movie);
                }

                return movies;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching movies from TMDB");
                throw;
            }
        }

        public async Task<Movie> GetMovieDetailsAsync(int movieId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/movie/{movieId}?append_to_response=credits");
                var content = await response.Content.ReadAsStringAsync();
                _logger.LogInformation("TMDB API Response for movie details: {Content}", content);

                response.EnsureSuccessStatusCode();
                var dto = JsonSerializer.Deserialize<TMDBMovieDTO>(content);
                
                if (dto == null)
                {
                    throw new Exception($"Movie with ID {movieId} not found");
                }

                var movie = dto.ToMovie();
                
                // If we have genres directly from the API, use those
                if (dto.Genres != null && dto.Genres.Any())
                {
                    movie.Genres = dto.Genres.Select(g => g.Name).ToList();
                    _logger.LogInformation("Using genres directly from API: {Genres}", string.Join(", ", movie.Genres));
                }
                // Otherwise, try to get genre names from genre IDs
                else if (dto.GenreIds != null && dto.GenreIds.Any())
                {
                    movie.Genres = await GetGenreNames(dto.GenreIds);
                    _logger.LogInformation("Converted genre IDs to names: {Genres}", string.Join(", ", movie.Genres));
                }

                // Add cast information
                if (dto.Credits?.Cast != null)
                {
                    _logger.LogInformation("Raw cast data from API: {Cast}", 
                        string.Join(", ", dto.Credits.Cast.Select(c => $"{c.Name} as {c.Character}")));
                    
                    movie.Cast = dto.Credits.Cast
                        .OrderBy(c => c.Order)
                        .Take(5) // Get top 5 cast members
                        .Select(c => new CastMember
                        {
                            Id = c.Id,
                            Name = c.Name,
                            Character = c.Character,
                            ProfilePath = c.ProfilePath
                        })
                        .ToList();
                    _logger.LogInformation("Processed cast members: {Cast}", 
                        string.Join(", ", movie.Cast.Select(c => $"{c.Name} as {c.Character}")));
                }
                else
                {
                    _logger.LogWarning("No cast data found in the API response");
                }
                
                return movie;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching movie details from TMDB");
                throw;
            }
        }

        private async Task<List<string>> GetGenreNames(List<int> genreIds)
        {
            if (genreIds == null || genreIds.Count == 0)
            {
                _logger.LogInformation("No genre IDs to convert");
                return new List<string>();
            }

            _logger.LogInformation("Converting genre IDs: {GenreIds}", string.Join(", ", genreIds));
            
            // Get all genre names in parallel
            var genreNameTasks = genreIds.Select(async genreId =>
            {
                var genreName = await _genreService.GetGenreName(genreId);
                _logger.LogInformation("Genre ID {GenreId} converted to name: {GenreName}", genreId, genreName);
                return genreName;
            });

            var genreNames = (await Task.WhenAll(genreNameTasks))
                .Where(name => !string.IsNullOrEmpty(name))
                .ToList();

            _logger.LogInformation("Final genre names for movie: {GenreNames}", string.Join(", ", genreNames));
            return genreNames;
        }
    }
} 