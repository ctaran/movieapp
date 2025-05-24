using System;
using Microsoft.AspNetCore.Identity;

namespace MovieApp.Core.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string UserId { get; set; } = string.Empty;
        public int MovieId { get; set; }
        public IdentityUser User { get; set; } = null!;
    }
} 