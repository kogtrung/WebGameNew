using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebGame.Models
{
    public class Achievement
    {
        public int Id { get; set; }
        [Required]
        public int GameId { get; set; }
        [ForeignKey("GameId")]
        public Game Game { get; set; }
        [Required]
        [StringLength(100)]
        public string Title { get; set; }
        [StringLength(300)]
        public string Description { get; set; }
        public string IconUrl { get; set; }
        public int Points { get; set; } = 0;
    }
} 