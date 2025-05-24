using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MovieApp.Core.Interfaces;
using MovieApp.Core.Models;

namespace MovieApp.Application.Services
{
    public class CommentService
    {
        private readonly ICommentRepository _commentRepository;

        public CommentService(ICommentRepository commentRepository)
        {
            _commentRepository = commentRepository;
        }

        public async Task<IEnumerable<Comment>> GetCommentsByMovieIdAsync(int movieId)
        {
            return await _commentRepository.GetCommentsByMovieIdAsync(movieId);
        }

        public async Task<Comment?> GetCommentByIdAsync(int id)
        {
            return await _commentRepository.GetCommentByIdAsync(id);
        }

        public async Task<Comment> AddCommentAsync(Comment comment)
        {
            comment.CreatedAt = DateTime.UtcNow;
            return await _commentRepository.AddCommentAsync(comment);
        }

        public async Task<Comment> UpdateCommentAsync(Comment comment)
        {
            var existingComment = await _commentRepository.GetCommentByIdAsync(comment.Id);
            if (existingComment == null)
            {
                throw new ArgumentException($"Comment with ID {comment.Id} not found");
            }

            existingComment.Content = comment.Content;
            existingComment.UpdatedAt = DateTime.UtcNow;

            return await _commentRepository.UpdateCommentAsync(existingComment);
        }

        public async Task DeleteCommentAsync(int id)
        {
            var comment = await _commentRepository.GetCommentByIdAsync(id);
            if (comment == null)
            {
                throw new ArgumentException($"Comment with ID {id} not found");
            }

            await _commentRepository.DeleteCommentAsync(id);
        }
    }
} 