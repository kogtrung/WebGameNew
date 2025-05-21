using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace WebGame.Models
{
    public class Achievement
    {
        public int Id { get; set; }
        [Required]
        public int GameId { get; set; }
        [ForeignKey("GameId")]
        [ValidateNever]
        public Game Game { get; set; }
        [Required]
        [StringLength(100)]
        public string Title { get; set; }
        [Required]
        [StringLength(300)]
        public string Description { get; set; }
        public string IconUrl { get; set; }
        public int Points { get; set; } = 0;

        // Thêm các trường mới để theo dõi điều kiện unlock
        [Required]
        public string ConditionType { get; set; } // Ví dụ: "Score", "PlayTime", "KillCount", etc.
        [Required]
        public int ConditionValue { get; set; } // Giá trị cần đạt để unlock
        public bool IsSecret { get; set; } = false; // Achievement bí mật (chỉ hiện khi unlock)
        public int DisplayOrder { get; set; } = 0; // Thứ tự hiển thị
    }
} 