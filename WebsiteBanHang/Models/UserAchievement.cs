using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebGame.Models
{
    public class UserAchievement
    {
        public int Id { get; set; }
        [Required]
        public int AchievementId { get; set; }
        [ForeignKey("AchievementId")]
        public Achievement Achievement { get; set; }
        [Required]
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }
        public DateTime UnlockedAt { get; set; } = DateTime.Now;
    }
} 