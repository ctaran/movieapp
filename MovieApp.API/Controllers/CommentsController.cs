using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MovieApp.Core.Interfaces;
using MovieApp.Core.Models;
using System.Security.Claims;

namespace MovieApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class CommentsController : ControllerBase
    {
        private readonly ICommentRepository _commentRepository;
        private readonly IMovieService _movieService;
        private readonly ILogger<CommentsController> _logger;

        public CommentsController(
            ICommentRepository commentRepository, 
            IMovieService movieService,
            ILogger<CommentsController> logger)
        {
            _commentRepository = commentRepository;
            _movieService = movieService;
            _logger = logger;
        }

        [HttpGet("movie/{movieId}")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<Comment>>> GetCommentsByMovie(int movieId)
        {
            try
            {
                var comments = await _commentRepository.GetCommentsByMovieIdAsync(movieId);
                return Ok(comments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting comments for movie {MovieId}", movieId);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<Comment>> GetComment(int id)
        {
            try
            {
                var comment = await _commentRepository.GetCommentByIdAsync(id);
                if (comment == null)
                {
                    return NotFound();
                }
                return Ok(comment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting comment {CommentId}", id);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<ActionResult<Comment>> CreateComment(Comment comment)
        {
            try
            {
                _logger.LogInformation("CreateComment called. User claims: {Claims}", 
                    string.Join(", ", User.Claims.Select(c => $"{c.Type}: {c.Value}")));

                if (string.IsNullOrWhiteSpace(comment.Content))
                {
                    return BadRequest("Comment content is required");
                }

                var movie = await _movieService.GetMovieDetailsAsync(comment.MovieId);
                if (movie == null)
                {
                    return BadRequest("Invalid movie ID");
                }

                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                _logger.LogInformation("User ID from token: {UserId}", userId);

                if (string.IsNullOrEmpty(userId))
                {
                    _logger.LogWarning("No user ID found in token");
                    return Unauthorized(new { message = "Invalid token" });
                }

                comment.UserId = userId;
                var createdComment = await _commentRepository.AddCommentAsync(comment);
                return CreatedAtAction(nameof(GetComment), new { id = createdComment.Id }, createdComment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating comment");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateComment(int id, Comment comment)
        {
            try
            {
                if (id != comment.Id)
                {
                    return BadRequest("Comment ID mismatch");
                }

                var existingComment = await _commentRepository.GetCommentByIdAsync(id);
                if (existingComment == null)
                {
                    return NotFound();
                }

                if (existingComment.UserId != User.FindFirst(ClaimTypes.NameIdentifier)?.Value)
                {
                    return Forbid();
                }

                await _commentRepository.UpdateCommentAsync(comment);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating comment {CommentId}", id);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteComment(int id)
        {
            try
            {
                var comment = await _commentRepository.GetCommentByIdAsync(id);
                if (comment == null)
                {
                    return NotFound();
                }

                if (comment.UserId != User.FindFirst(ClaimTypes.NameIdentifier)?.Value)
                {
                    return Forbid();
                }

                await _commentRepository.DeleteCommentAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting comment {CommentId}", id);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
} 