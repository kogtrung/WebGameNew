using System;

namespace WebGame.Models
{
    public class ForumReport
    {
        public int Id { get; set; }
        public int? ForumPostId { get; set; }
        public ForumPost ForumPost { get; set; }
        public int? ForumCommentId { get; set; }
        public ForumComment ForumComment { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public string Reason { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public bool IsResolved { get; set; } = false;
    }
} 