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
    public class GameNotificationController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public GameNotificationController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: GameNotification
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Challenge(); // Require login
            }

            var notifications = await _context.GameNotifications
                .Include(gn => gn.Game)
                .Where(gn => gn.UserId == user.Id)
                .OrderByDescending(gn => gn.CreatedAt)
                .ToListAsync();

            // Mark notifications as read when user views them
            foreach (var notification in notifications.Where(n => !n.IsRead))
            {
                notification.IsRead = true;
            }
            await _context.SaveChangesAsync();

            return View(notifications);
        }

        // POST: GameNotification/MarkAsRead/5
        [HttpPost]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            var notification = await _context.GameNotifications.FirstOrDefaultAsync(n => n.Id == id && n.UserId == user.Id);

            if (notification != null && !notification.IsRead)
            {
                notification.IsRead = true;
                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }

            return Json(new { success = false, message = "Notification not found or already read." });
        }

        // POST: GameNotification/MarkAllAsRead
        [HttpPost]
        public async Task<IActionResult> MarkAllAsRead()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            var unreadNotifications = await _context.GameNotifications
                .Where(n => n.UserId == user.Id && !n.IsRead)
                .ToListAsync();

            foreach (var notification in unreadNotifications)
            {
                notification.IsRead = true;
            }

            await _context.SaveChangesAsync();
            return Json(new { success = true, count = unreadNotifications.Count });
        }
    }
} 