using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using WebGame.Models;

namespace WebGame.Controllers
{
    public class AchievementController : Controller
    {
        private readonly ApplicationDbContext _context;
        public AchievementController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Hiển thị danh sách thành tựu của game
        [HttpGet]
        public async Task<IActionResult> Index(int gameId)
        {
            var game = await _context.Games.Include(g => g.Achievements).FirstOrDefaultAsync(g => g.Id == gameId);
            if (game == null) return NotFound();
            var achievements = await _context.Achievements.Where(a => a.GameId == gameId).ToListAsync();
            var userId = User.Identity.IsAuthenticated ? User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.NameIdentifier)?.Value : null;
            var unlocked = userId != null ? _context.UserAchievements.Where(ua => ua.UserId == userId && ua.Achievement.GameId == gameId).Select(ua => ua.AchievementId).ToList() : new List<int>();
            ViewBag.Game = game;
            ViewBag.Unlocked = unlocked;
            return View(achievements);
        }

        // Admin: Thêm thành tựu
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Create(int gameId)
        {
            ViewBag.GameId = gameId;
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Achievement achievement)
        {
            if (ModelState.IsValid)
            {
                _context.Achievements.Add(achievement);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", new { gameId = achievement.GameId });
            }
            ViewBag.GameId = achievement.GameId;
            return View(achievement);
        }

        // Admin: Sửa thành tựu
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var achievement = await _context.Achievements.FindAsync(id);
            if (achievement == null) return NotFound();
            return View(achievement);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Achievement achievement)
        {
            if (ModelState.IsValid)
            {
                _context.Achievements.Update(achievement);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", new { gameId = achievement.GameId });
            }
            return View(achievement);
        }

        // Admin: Xóa thành tựu
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var achievement = await _context.Achievements.FindAsync(id);
            if (achievement != null)
            {
                _context.Achievements.Remove(achievement);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", new { gameId = achievement.GameId });
            }
            return NotFound();
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Unlock(int achievementId)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return Unauthorized();
            var exists = await _context.UserAchievements.AnyAsync(ua => ua.AchievementId == achievementId && ua.UserId == userId);
            if (!exists)
            {
                var userAchievement = new UserAchievement
                {
                    AchievementId = achievementId,
                    UserId = userId,
                    UnlockedAt = DateTime.Now
                };
                _context.UserAchievements.Add(userAchievement);
                await _context.SaveChangesAsync();
            }
            var achievement = await _context.Achievements.FindAsync(achievementId);
            return RedirectToAction("Index", new { gameId = achievement?.GameId });
        }
    }
} 