using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebGame.Models;

namespace WebGame.Controllers
{
    [Authorize]
    public class NotificationsController : Controller
    {
        private readonly ApplicationDbContext _context;
        public NotificationsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            // Lấy thông báo diễn đàn
            var forumNotifications = await _context.ForumNotifications
                .Where(n => n.UserId == userId)
                .Select(n => new UnifiedNotificationViewModel
                {
                    Id = n.Id,
                    Message = n.Message,
                    CreatedAt = n.CreatedAt,
                    IsRead = n.IsRead,
                    Type = "Forum",
                    Link = n.ForumPostId != null ? Url.Action("Details", "Forum", new { id = n.ForumPostId }) : null
                }).ToListAsync();

            // Lấy thông báo game (nếu có bảng GameNotification)
            var gameNotifications = _context.GameNotifications != null
                ? await _context.GameNotifications
                    .Where(n => n.UserId == userId)
                    .Select(n => new UnifiedNotificationViewModel
                    {
                        Id = n.Id,
                        Message = n.Message,
                        CreatedAt = n.CreatedAt,
                        IsRead = n.IsRead,
                        Type = "Game",
                        Link = n.GameId != null ? Url.Action("Details", "Game", new { id = n.GameId }) : null
                    }).ToListAsync()
                : new List<UnifiedNotificationViewModel>();

            // Gộp và sắp xếp
            var allNotifications = forumNotifications.Concat(gameNotifications)
                .OrderByDescending(n => n.CreatedAt)
                .ToList();

            return View(allNotifications);
        }
    }

    public class UnifiedNotificationViewModel
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsRead { get; set; }
        public string Type { get; set; } // Forum/Game
        public string Link { get; set; }
    }
} 