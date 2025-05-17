using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebGame.Models;
using System.Linq;
using System.Threading.Tasks;

namespace WebsiteBanHang.Controllers
{
    [Authorize]
    public class GameFollowController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public GameFollowController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: GameFollow
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Challenge(); // Require login
            }

            var followedGames = await _context.GameFollows
                .Include(gf => gf.Game)
                .ThenInclude(g => g.GamePlatforms)
                .ThenInclude(gp => gp.Platform)
                .Where(gf => gf.UserId == user.Id)
                .Select(gf => gf.Game) // Select the Game entity
                .ToListAsync();

            return View(followedGames);
        }

        // POST: GameFollow/Follow/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Follow(int id)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Json(new { success = false, message = "You must be logged in to follow games." });
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Json(new { success = false, message = "User not found." });
            }

            var existingFollow = await _context.GameFollows
                .FirstOrDefaultAsync(gf => gf.UserId == user.Id && gf.GameId == id);

            if (existingFollow != null)
            {
                return Json(new { success = false, message = "You are already following this game." });
            }

            var gameFollow = new GameFollow
            {
                UserId = user.Id,
                GameId = id
            };

            _context.GameFollows.Add(gameFollow);
            await _context.SaveChangesAsync();

            // Update followers count (optional, but good for realism)
            var game = await _context.Games.FindAsync(id);
            if (game != null)
            {
                game.FollowersCount++;
                _context.Games.Update(game);
                await _context.SaveChangesAsync();
            }

            return Json(new { success = true, message = "Game followed successfully." });
        }

        // POST: GameFollow/Unfollow/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Unfollow(int id)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Json(new { success = false, message = "You must be logged in to unfollow games." });
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Json(new { success = false, message = "User not found." });
            }

            var existingFollow = await _context.GameFollows
                .FirstOrDefaultAsync(gf => gf.UserId == user.Id && gf.GameId == id);

            if (existingFollow == null)
            {
                return Json(new { success = false, message = "You are not following this game." });
            }

            _context.GameFollows.Remove(existingFollow);
            await _context.SaveChangesAsync();

            // Decrease followers count (optional)
            var game = await _context.Games.FindAsync(id);
            if (game != null)
            {
                game.FollowersCount--;
                // Ensure followers count doesn't go below zero
                if (game.FollowersCount < 0) game.FollowersCount = 0;
                _context.Games.Update(game);
                await _context.SaveChangesAsync();
            }

            return Json(new { success = true, message = "Game unfollowed successfully." });
        }

        // You might want actions to manage notification settings later
    }
} 