using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebGame.Models;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace WebGame.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ForumReportController : Controller
    {
        private readonly ApplicationDbContext _context;
        public ForumReportController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Danh sách báo cáo
        public async Task<IActionResult> Index()
        {
            var reports = await _context.ForumReports
                .Include(r => r.User)
                .Include(r => r.ForumPost)
                .Include(r => r.ForumComment)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
            return View(reports);
        }

        // Đánh dấu đã xử lý
        [HttpPost]
        public async Task<IActionResult> Resolve(int id)
        {
            var report = await _context.ForumReports
                .Include(r => r.ForumPost)
                .Include(r => r.ForumComment)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (report != null)
            {
                report.IsResolved = true;
                await _context.SaveChangesAsync();

                // Tạo thông báo cho người báo cáo
                var notification = new ForumNotification
                {
                    UserId = report.UserId,
                    Message = $"Báo cáo của bạn đã được xử lý",
                    Type = "ReportResolved",
                    ForumPostId = report.ForumPostId,
                    ForumCommentId = report.ForumCommentId,
                    CreatedAt = DateTime.Now
                };
                _context.ForumNotifications.Add(notification);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // Xóa bài viết vi phạm
        [HttpPost]
        public async Task<IActionResult> DeletePost(int postId)
        {
            // Xóa tất cả notification liên quan trước
            var notifications = _context.ForumNotifications.Where(n => n.ForumPostId == postId);
            _context.ForumNotifications.RemoveRange(notifications);
            await _context.SaveChangesAsync();

            var post = await _context.ForumPosts
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.Id == postId);

            if (post != null)
            {
                // Xóa tất cả report liên quan trước
                var reports = await _context.ForumReports
                    .Where(r => r.ForumPostId == postId)
                    .ToListAsync();

                foreach (var report in reports)
                {
                    var notification = new ForumNotification
                    {
                        UserId = report.UserId,
                        Message = $"Bài viết bạn báo cáo đã bị xóa",
                        Type = "ReportResolved",
                        ForumPostId = postId,
                        CreatedAt = DateTime.Now
                    };
                    _context.ForumNotifications.Add(notification);
                }

                _context.ForumReports.RemoveRange(reports);
                _context.ForumPosts.Remove(post);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // Xóa bình luận vi phạm
        [HttpPost]
        public async Task<IActionResult> DeleteComment(int commentId)
        {
            // Xóa tất cả notification liên quan trước
            var notifications = _context.ForumNotifications.Where(n => n.ForumCommentId == commentId);
            _context.ForumNotifications.RemoveRange(notifications);
            await _context.SaveChangesAsync();

            var comment = await _context.ForumComments
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.Id == commentId);

            if (comment != null)
            {
                // Xóa tất cả report liên quan trước
                var reports = await _context.ForumReports
                    .Where(r => r.ForumCommentId == commentId)
                    .ToListAsync();

                foreach (var report in reports)
                {
                    var notification = new ForumNotification
                    {
                        UserId = report.UserId,
                        Message = $"Bình luận bạn báo cáo đã bị xóa",
                        Type = "ReportResolved",
                        ForumCommentId = commentId,
                        CreatedAt = DateTime.Now
                    };
                    _context.ForumNotifications.Add(notification);
                }

                _context.ForumReports.RemoveRange(reports);
                _context.ForumComments.Remove(comment);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
} 