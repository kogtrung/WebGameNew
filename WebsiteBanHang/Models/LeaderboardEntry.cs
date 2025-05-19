using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebGame.Models
{
    public class LeaderboardEntry
    {
        public int Id { get; set; }
        [Required]
        public int GameId { get; set; }
        [ForeignKey("GameId")]
        public Game Game { get; set; }
        [Required]
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }
        public int Score { get; set; } = 0;
        public int PlayTime { get; set; } = 0; // minutes
        public int Rank { get; set; } = 0;
        public DateTime LastUpdate { get; set; } = DateTime.Now;
    }
} 