using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebGame.Models;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace WebsiteBanHang.Controllers
{
    [Authorize]
    public class GameComparisonController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public GameComparisonController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: GameComparison
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Challenge(); // Require login
            }

            var comparisons = await _context.GameComparisons
                .Where(gc => gc.UserId == user.Id)
                .OrderByDescending(gc => gc.CreatedAt)
                .ToListAsync();

            return View(comparisons);
        }

        // GET: GameComparison/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var comparison = await _context.GameComparisons
                .Include(gc => gc.Games)
                .ThenInclude(gci => gci.Game)
                .ThenInclude(g => g.GamePlatforms)
                .ThenInclude(gp => gp.Platform)
                .Include(gc => gc.Games)
                .ThenInclude(gci => gci.Game)
                 .ThenInclude(g => g.Reviews) // Include reviews for user score
                .FirstOrDefaultAsync(m => m.Id == id);

            if (comparison == null)
            {
                return NotFound();
            }
            
            // Check if the comparison is public or if the current user is the owner
            var currentUser = await _userManager.GetUserAsync(User);
            if (!comparison.IsPublic && comparison.UserId != currentUser?.Id)
            {
                 return Forbid(); // User is not authorized to view this private comparison
            }

            return View(comparison);
        }

        // GET: GameComparison/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: GameComparison/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,IsPublic")] GameComparison gameComparison)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            gameComparison.UserId = user.Id;
            gameComparison.CreatedAt = DateTime.Now;
            gameComparison.LastUpdated = DateTime.Now;

            if (ModelState.IsValid)
            {
                _context.Add(gameComparison);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Details), new { id = gameComparison.Id });
            }
            return View(gameComparison);
        }

        // GET: GameComparison/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var gameComparison = await _context.GameComparisons
                 .Where(gc => gc.Id == id)
                 .Include(gc => gc.Games)
                 .ThenInclude(gci => gci.Game)
                 .FirstOrDefaultAsync();

            if (gameComparison == null)
            {
                return NotFound();
            }
            
            var user = await _userManager.GetUserAsync(User);
            if (gameComparison.UserId != user?.Id)
            {
                return Forbid(); // Only the owner can edit
            }

            return View(gameComparison);
        }

        // POST: GameComparison/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,IsPublic")] GameComparison gameComparison)
        {
            if (id != gameComparison.Id)
            {
                return NotFound();
            }
             var user = await _userManager.GetUserAsync(User);
            if (gameComparison.UserId != user?.Id)
            {
                return Forbid(); // Only the owner can edit
            }

            if (ModelState.IsValid)
            {
                try
                {
                    gameComparison.LastUpdated = DateTime.Now;
                    _context.Update(gameComparison);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GameComparisonExists(gameComparison.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(gameComparison);
        }

        // POST: GameComparison/AddItem
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddItem(int comparisonId, int gameId)
        {
             var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            var comparison = await _context.GameComparisons
                .Include(gc => gc.Games)
                .FirstOrDefaultAsync(gc => gc.Id == comparisonId && gc.UserId == user.Id);

            if (comparison == null)
            {
                return NotFound(); // Comparison not found or not owned by user
            }

            var game = await _context.Games.FindAsync(gameId);
            if (game == null)
            {
                return NotFound(); // Game not found
            }

            // Check if game is already in comparison
            if (comparison.Games.Any(gci => gci.GameId == gameId))
            {
                 return Json(new { success = false, message = "Game này đã có trong bảng so sánh." });
            }

            var comparisonItem = new GameComparisonItem
            {
                ComparisonId = comparisonId,
                GameId = gameId,
                Order = comparison.Games.Count // Add to the end
            };

            _context.GameComparisonItems.Add(comparisonItem);
            comparison.LastUpdated = DateTime.Now; // Update comparison last updated time
            await _context.SaveChangesAsync();

             return Json(new { success = true, message = "Đã thêm game vào bảng so sánh.", itemId = comparisonItem.Id });
        }

         // POST: GameComparison/RemoveItem
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveItem(int itemId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            var comparisonItem = await _context.GameComparisonItems
                .Include(gci => gci.Comparison)
                .FirstOrDefaultAsync(gci => gci.Id == itemId);

            if (comparisonItem == null)
            {
                return NotFound();
            }
            
            // Ensure the user owns the comparison this item belongs to
            if(comparisonItem.Comparison.UserId != user.Id)
            {
                return Forbid();
            }

            _context.GameComparisonItems.Remove(comparisonItem);
             comparisonItem.Comparison.LastUpdated = DateTime.Now; // Update comparison last updated time
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Đã xóa game khỏi bảng so sánh." });
        }


        // POST: GameComparison/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var gameComparison = await _context.GameComparisons.FindAsync(id);
            var user = await _userManager.GetUserAsync(User);

            if (gameComparison != null && gameComparison.UserId == user?.Id)
            {
                _context.GameComparisons.Remove(gameComparison);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return NotFound(); // Not found or not owned by user
        }

        private bool GameComparisonExists(int id)
        {
            return _context.GameComparisons.Any(e => e.Id == id);
        }

        // Helper to calculate user score for a game
        private decimal? CalculateUserScore(Game game)
        {
            if (game?.Reviews == null || !game.Reviews.Any()) return null;
            return Math.Round(game.Reviews.Average(r => r.Score), 1);
        }
    }
} 