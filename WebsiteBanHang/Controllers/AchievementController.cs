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

            var userId = User.Identity.IsAuthenticated ? User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.NameIdentifier)?.Value : null;
            var unlocked = userId != null ? 
                await _context.UserAchievements
                    .Where(ua => ua.UserId == userId && ua.Achievement.GameId == gameId)
                    .Select(ua => ua.AchievementId)
                    .ToListAsync() 
                : new List<int>();

            // Lấy achievements, ẩn các achievement bí mật chưa unlock
            var achievements = await _context.Achievements
                .Where(a => a.GameId == gameId && (!a.IsSecret || unlocked.Contains(a.Id)))
                .OrderBy(a => a.DisplayOrder)
                .ToListAsync();

            ViewBag.Game = game;
            ViewBag.Unlocked = unlocked;
            return View(achievements);
        }

        // API để kiểm tra và unlock achievements
        [HttpPost]
        [Authorize]
        [Route("api/achievements/check")]
        public async Task<IActionResult> CheckAchievements(int gameId, string conditionType, int value)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            // Lấy tất cả achievements của game
            var achievements = await _context.Achievements
                .Where(a => a.GameId == gameId && a.ConditionType == conditionType)
                .ToListAsync();

            // Lấy danh sách achievements đã unlock
            var unlocked = await _context.UserAchievements
                .Where(ua => ua.UserId == userId && ua.Achievement.GameId == gameId)
                .Select(ua => ua.AchievementId)
                .ToListAsync();

            var newlyUnlocked = new List<Achievement>();

            foreach (var achievement in achievements)
            {
                // Kiểm tra nếu chưa unlock và đạt điều kiện
                if (!unlocked.Contains(achievement.Id) && value >= achievement.ConditionValue)
                {
                    var userAchievement = new UserAchievement
                    {
                        AchievementId = achievement.Id,
                        UserId = userId,
                        UnlockedAt = DateTime.Now
                    };
                    _context.UserAchievements.Add(userAchievement);
                    newlyUnlocked.Add(achievement);
                }
            }

            if (newlyUnlocked.Any())
            {
                await _context.SaveChangesAsync();
            }

            return Json(new { 
                success = true, 
                newlyUnlocked = newlyUnlocked.Select(a => new { 
                    id = a.Id, 
                    title = a.Title, 
                    description = a.Description,
                    points = a.Points
                })
            });
        }

        // API để lấy thông tin achievement
        [HttpGet]
        [Authorize]
        [Route("api/achievements/{id}")]
        public async Task<IActionResult> GetAchievement(int id)
        {
            var achievement = await _context.Achievements.FindAsync(id);
            if (achievement == null) return NotFound();

            return Json(new {
                id = achievement.Id,
                title = achievement.Title,
                description = achievement.Description,
                points = achievement.Points,
                conditionType = achievement.ConditionType,
                conditionValue = achievement.ConditionValue,
                isSecret = achievement.IsSecret
            });
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
            if (!ModelState.IsValid)
            {
                // Debug lỗi
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                ViewBag.Errors = errors;
                ViewBag.GameId = achievement.GameId;
                return View(achievement);
            }
            _context.Achievements.Add(achievement);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", new { gameId = achievement.GameId });
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