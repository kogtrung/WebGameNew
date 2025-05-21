using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebGame.Models
{
    public class ForumComment
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public string UserId { get; set; }
        public int ForumPostId { get; set; }
        public ForumPost ForumPost { get; set; }
        public ApplicationUser User { get; set; }
        public ICollection<ForumCommentVote> Votes { get; set; }
        public string? QuoteContent { get; set; }
    }

}