using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using System.Linq;

namespace WebGame.Models
{
    public class Game
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; }

        public string Platform 
        { 
            get 
            {
                if (GamePlatforms != null)
                {
                    List<string> platformNames = new List<string>();
                    foreach (var gp in GamePlatforms)
                    {
                        if (gp.Platform != null)
                        {
                            platformNames.Add(gp.Platform.Name);
                        }
                        else
                        {
                            platformNames.Add("Unknown");
                        }
                    }
                    
                    if (platformNames.Count > 0)
                    {
                        return string.Join(", ", platformNames);
                    }
                }
                return "Not specified";
            }
            set { }
        }

        [Required]
        public string Description { get; set; }

        public string? ImageUrl { get; set; }
        
        public string? VideoUrl { get; set; }
        
        public string? TrailerUrl { get; set; }
        
        public List<string>? Screenshots { get; set; }

        [Required]
        [Range(0, 100)]
        public int MetaScore { get; set; }

        [DataType(DataType.Date)]
        public DateTime? ReleaseDate { get; set; }

        [Required]
        public string Genre { get; set; }

        [Required]
        public string Developer { get; set; }

        [Required]
        public string Publisher { get; set; }

        // Review statistics
        public int UserScore { get; set; }
        public int ReviewCount { get; set; }
        public int UserReviewCount { get; set; }

        public float TrendingScore { get; set; }

        public string? Screenshot { get; set; }

        [Required]
        public string Rating { get; set; }

        public bool MustPlay { get; set; }

        // New fields for tracking and notifications
        public int FollowersCount { get; set; } = 0;
        public DateTime? LastUpdateDate { get; set; }
        public string? UpdateNotes { get; set; }
        public decimal? PreviousPrice { get; set; }
        public DateTime? PriceChangeDate { get; set; }
        public bool IsOnSale { get; set; } = false;
        public DateTime? SaleEndDate { get; set; }
        public int DiscountPercentage { get; set; } = 0;

        // Navigation properties
        public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
        public virtual ICollection<GamePlatform> GamePlatforms { get; set; } = new List<GamePlatform>();
        public virtual ICollection<GameFollow> Followers { get; set; } = new List<GameFollow>();
        public virtual ICollection<GameNotification> Notifications { get; set; } = new List<GameNotification>();
    }

    public class GameFollow
    {
        public int Id { get; set; }
        
        [Required]
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }
        
        [Required]
        public int GameId { get; set; }
        [ForeignKey("GameId")]
        public Game Game { get; set; }
        
        public DateTime FollowDate { get; set; } = DateTime.Now;
        
        public bool NotifyOnUpdates { get; set; } = true;
        
        public bool NotifyOnPriceChanges { get; set; } = true;
        
        public bool NotifyOnNewReviews { get; set; } = true;
    }

    public class GameNotification
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
        
        public string Title { get; set; }
        
        public string Message { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        
        public bool IsRead { get; set; } = false;
        
        public NotificationType Type { get; set; }
    }

    public enum NotificationType
    {
        Update,
        PriceChange,
        NewReview,
        Sale
    }
} 