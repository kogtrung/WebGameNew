using System.ComponentModel.DataAnnotations;

namespace WebGame.Models
{
    public class ForumCommentVote
    {
        public int Id { get; set; }
        public int ForumCommentId { get; set; }
        public ForumComment ForumComment { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public int Value { get; set; } // 1: upvote, -1: downvote
    }
} 