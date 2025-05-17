using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebGame.Models
{
    public class Review
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }

        [Required]
        public int GameId { get; set; }
        [ForeignKey("GameId")]
        public Game Game { get; set; }

        [Required]
        [Range(0, 10)]
        public decimal Score { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }

        [Required]
        public DateTime ReviewDate { get; set; } = DateTime.Now;

        public bool IsCriticReview { get; set; } = false;

        public string CriticName { get; set; }

        public string CriticWebsite { get; set; }

        public int Upvotes { get; set; } = 0;

        public int Downvotes { get; set; } = 0;

        public ReviewType ReviewType { get; set; } = ReviewType.Mixed;

        public bool IsVerifiedPurchase { get; set; } = false;

        public int PlayTimeHours { get; set; } = 0;

        public bool IsHelpful { get; set; } = false;

        public int HelpfulCount { get; set; } = 0;
        public int UnhelpfulCount { get; set; } = 0;
    }

    public enum ReviewType
    {
        Positive,
        Mixed,
        Negative
    }
} 