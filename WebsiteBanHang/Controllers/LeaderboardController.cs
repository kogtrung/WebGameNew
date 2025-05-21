using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using WebGame.Models;
using Microsoft.AspNetCore.Authorization;

namespace WebGame.Controllers
{
    public class LeaderboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        public LeaderboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Hiển thị bảng xếp hạng của game
        [HttpGet]
        public async Task<IActionResult> Index(int gameId)
        {
            var game = await _context.Games.FirstOrDefaultAsync(g => g.Id == gameId);
            if (game == null) return NotFound();
            var leaderboard = await _context.LeaderboardEntries
                .Include(e => e.User)
                .Where(e => e.GameId == gameId)
                .OrderByDescending(e => e.Score)
                .ThenBy(e => e.PlayTime)
                .Take(100)
                .ToListAsync();
            ViewBag.Game = game;
            return View(leaderboard);
        }

        [HttpGet]
        [Authorize]
        public IActionResult SubmitScore(int gameId)
        {
            ViewBag.GameId = gameId;
            return View();
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitScore(int gameId, int score, int playTime)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return Unauthorized();
            // Kiểm tra đã có entry chưa
            var entry = await _context.LeaderboardEntries.FirstOrDefaultAsync(e => e.GameId == gameId && e.UserId == userId);
            if (entry == null)
            {
                entry = new LeaderboardEntry
                {
                    GameId = gameId,
                    UserId = userId,
                    Score = score,
                    PlayTime = playTime,
                    LastUpdate = DateTime.Now
                };
                _context.LeaderboardEntries.Add(entry);
            }
            else
            {
                entry.Score = score;
                entry.PlayTime = playTime;
                entry.LastUpdate = DateTime.Now;
                _context.LeaderboardEntries.Update(entry);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", new { gameId });
        }
    }
} 