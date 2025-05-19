using System.ComponentModel.DataAnnotations;

namespace WebGame.Models
{
    public class ForumPostViewModel
    {
        [Required]
        public string Title { get; set; }
        [Required]
        public string Content { get; set; }
        [Required]
        public int CategoryId { get; set; }
    }
} 