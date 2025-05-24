using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MovieApp.Core.Interfaces;
using MovieApp.Core.Models;

namespace MovieApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MoviesController : ControllerBase
    {
        private readonly IMovieService _movieService;
        private readonly IGenreService _genreService;

        public MoviesController(IMovieService movieService, IGenreService genreService)
        {
            _movieService = movieService;
            _genreService = genreService;
        }

        [HttpGet("latest")]
        public async Task<ActionResult<IEnumerable<Movie>>> GetLatestMovies()
        {
            try
            {
                var movies = await _movieService.GetLatestMoviesAsync();
                return Ok(movies);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("top-rated")]
        public async Task<ActionResult<IEnumerable<Movie>>> GetTopRatedMovies()
        {
            try
            {
                var movies = await _movieService.GetTopRatedMoviesAsync();
                return Ok(movies);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Search movies by name and/or genre
        /// </summary>
        /// <param name="query">Optional search term for movie name</param>
        /// <param name="genre">Optional genre name (e.g., "action", "comedy", "drama")</param>
        /// <returns>List of movies matching the search criteria</returns>
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Movie>>> SearchMovies([FromQuery] string? query, [FromQuery] string? genre)
        {
            try
            {
                var movies = await _movieService.SearchMoviesAsync(query, genre);
                return Ok(movies);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Movie>> GetMovieDetails(int id)
        {
            try
            {
                var movie = await _movieService.GetMovieDetailsAsync(id);
                return Ok(movie);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Get all available genres
        /// </summary>
        /// <returns>List of available genres with their IDs</returns>
        [HttpGet("genres")]
        public async Task<ActionResult<IEnumerable<Genre>>> GetGenres()
        {
            try
            {
                var genres = await _genreService.GetAllGenres();
                return Ok(genres);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
} 