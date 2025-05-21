using System.ComponentModel.DataAnnotations;

namespace WebGame.Models
{
    public class ForumPostVote
    {
        public int Id { get; set; }
        public int ForumPostId { get; set; }
        public ForumPost ForumPost { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public int Value { get; set; } // 1: upvote, -1: downvote
    }
} 