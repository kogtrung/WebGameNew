using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebGame.Models
{
    public class ForumPost
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        [Required]
        public int CategoryId { get; set; }
        public ForumCategory Category { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public ICollection<ForumComment> Comments { get; set; }
        public string? ImageUrl { get; set; }
        public ICollection<ForumPostVote> Votes { get; set; }
    }
} 