using System;

namespace WebGame.Models
{
    public class ForumNotification
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public string Message { get; set; }
        public string Type { get; set; } // "Reply", "Report", "ReportResolved"
        public int? ForumPostId { get; set; }
        public ForumPost ForumPost { get; set; }
        public int? ForumCommentId { get; set; }
        public ForumComment ForumComment { get; set; }
        public bool IsRead { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
} 