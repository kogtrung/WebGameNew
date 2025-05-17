using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using WebGame.Models;

namespace WebGame.Models
{
    public class GameComparison
    {
        public int Id { get; set; }
        
        [Required]
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }
        
        [Required]
        [StringLength(200)]
        public string Name { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        
        public DateTime? LastUpdated { get; set; }
        
        public bool IsPublic { get; set; } = false;
        
        public virtual ICollection<GameComparisonItem> Games { get; set; } = new List<GameComparisonItem>();
    }
    
    public class GameComparisonItem
    {
        public int Id { get; set; }
        
        [Required]
        public int ComparisonId { get; set; }
        [ForeignKey("ComparisonId")]
        public GameComparison Comparison { get; set; } = null!;
        
        [Required]
        public int GameId { get; set; }
        [ForeignKey("GameId")]
        public Game Game { get; set; }
        
        public int Order { get; set; }
        
        [StringLength(500)]
        public string Notes { get; set; }
    }
    
    public class ComparisonFeature
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Name { get; set; }
        
        [StringLength(500)]
        public string Description { get; set; }
        
        public FeatureCategory Category { get; set; }
        
        public virtual ICollection<GameFeature> GameFeatures { get; set; } = new List<GameFeature>();
    }
    
    public class GameFeature
    {
        public int Id { get; set; }
        
        [Required]
        public int GameId { get; set; }
        [ForeignKey("GameId")]
        public Game Game { get; set; }
        
        [Required]
        public int FeatureId { get; set; }
        [ForeignKey("FeatureId")]
        public ComparisonFeature Feature { get; set; }
        
        [StringLength(200)]
        public string Value { get; set; }
        
        public bool IsAvailable { get; set; } = true;
    }
    
    public enum FeatureCategory
    {
        Technical,
        Gameplay,
        Content,
        Performance,
        Price,
        Rating
    }
} 