using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MovieApp.Core.Interfaces;
using MovieApp.Core.Models;
using System.Net.Http.Headers;
using System.Text.Json.Serialization;
using System.Linq;
using System.Threading;

namespace MovieApp.Infrastructure.Services
{
    public class TMDBGenreService : IGenreService
    {
        private readonly HttpClient _httpClient;
        private readonly string _accessToken;
        private readonly string _baseUrl = "https://api.themoviedb.org/3";
        private readonly ILogger<TMDBGenreService> _logger;
        private readonly Dictionary<int, string> _genreCache;
        private bool _isInitialized;
        private readonly SemaphoreSlim _initLock = new SemaphoreSlim(1, 1);

        public TMDBGenreService(
            HttpClient httpClient,
            IConfiguration configuration,
            ILogger<TMDBGenreService> logger)
        {
            _httpClient = httpClient;
            _accessToken = configuration["TMDB:AccessToken"] ?? throw new ArgumentNullException(nameof(configuration), "TMDB:AccessToken is not configured");
            _logger = logger;
            _genreCache = new Dictionary<int, string>();
            _isInitialized = false;
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
            _logger.LogInformation("TMDBGenreService initialized with access token: {TokenLength} chars", _accessToken.Length);
        }

        private async Task InitializeAsync()
        {
            if (_isInitialized) 
            {
                _logger.LogInformation("Genre service already initialized, skipping");
                return;
            }

            try
            {
                await _initLock.WaitAsync();
                if (_isInitialized) 
                {
                    _logger.LogInformation("Genre service already initialized after lock, skipping");
                    return;
                }

                _logger.LogInformation("Initializing genre service by fetching from TMDB API");
                var response = await _httpClient.GetAsync($"{_baseUrl}/genre/movie/list");
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                _logger.LogInformation("TMDB API Response for genres: {Content}", content);

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                var result = JsonSerializer.Deserialize<TMDBGenreResponse>(content, options);
                
                if (result?.Genres != null)
                {
                    _logger.LogInformation("Found {Count} genres in TMDB response", result.Genres.Count);
                    foreach (var genre in result.Genres)
                    {
                        if (genre.Id > 0 && !string.IsNullOrEmpty(genre.Name))
                        {
                            _genreCache[genre.Id] = genre.Name;
                            _logger.LogInformation("Cached genre: ID={Id}, Name={Name}", genre.Id, genre.Name);
                        }
                        else
                        {
                            _logger.LogWarning("Skipping invalid genre: ID={Id}, Name={Name}", genre.Id, genre.Name);
                        }
                    }
                    _logger.LogInformation("Genre cache now contains {Count} entries", _genreCache.Count);
                }
                else
                {
                    _logger.LogWarning("No genres found in TMDB response");
                }
                
                _isInitialized = true;
                _logger.LogInformation("Genre service initialization completed");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error initializing genres from TMDB");
                throw;
            }
            finally
            {
                _initLock.Release();
            }
        }

        public async Task<string?> GetGenreName(int genreId)
        {
            _logger.LogInformation("Getting genre name for ID: {GenreId}, IsInitialized: {IsInitialized}", genreId, _isInitialized);
            
            if (!_isInitialized)
            {
                _logger.LogInformation("Genre service not initialized, initializing now");
                await InitializeAsync();
            }

            var hasGenre = _genreCache.TryGetValue(genreId, out var name);
            _logger.LogInformation("Looking up genre ID {GenreId}: Found={Found}, Name={Name}, CacheSize={CacheSize}", 
                genreId, hasGenre, name, _genreCache.Count);
            return hasGenre ? name : null;
        }

        public async Task<int?> GetGenreId(string genreName)
        {
            if (string.IsNullOrWhiteSpace(genreName))
                return null;

            if (!_isInitialized)
            {
                await InitializeAsync();
            }

            var normalizedName = genreName.ToLower().Trim();
            var genre = _genreCache.FirstOrDefault(x => x.Value.ToLower() == normalizedName);
            return genre.Key != 0 ? genre.Key : null;
        }

        public async Task<IEnumerable<Genre>> GetAllGenres()
        {
            if (!_isInitialized)
            {
                await InitializeAsync();
            }

            return _genreCache.Select(kvp => new Genre { Id = kvp.Key, Name = kvp.Value });
        }
    }

    public class TMDBGenreResponse
    {
        [JsonPropertyName("genres")]
        public List<Genre> Genres { get; set; } = new();
    }
} 