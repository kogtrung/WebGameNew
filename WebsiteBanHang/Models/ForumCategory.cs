using System.Collections.Generic;

namespace WebGame.Models
{
    public class ForumCategory
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public ICollection<ForumPost> ForumPosts { get; set; }
    }
} 