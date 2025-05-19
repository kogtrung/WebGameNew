using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using WebGame.Models;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace WebGame.Controllers
{
    public class ForumController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ForumController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Forum
        public async Task<IActionResult> Index(int? categoryId)
        {
            var categories = await _context.ForumCategories.ToListAsync();
            var posts = _context.ForumPosts.Include(p => p.User).Include(p => p.Category).OrderByDescending(p => p.CreatedAt).AsQueryable();
            if (categoryId.HasValue)
            {
                posts = posts.Where(p => p.CategoryId == categoryId.Value);
            }
            ViewBag.Categories = categories;
            ViewBag.SelectedCategory = categoryId;
            return View(await posts.ToListAsync());
        }

        // GET: /Forum/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var post = await _context.ForumPosts
                .Include(p => p.User)
                .Include(p => p.Category)
                .Include(p => p.Comments).ThenInclude(c => c.User)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (post == null) return NotFound();
            return View(post);
        }

        // GET: /Forum/Create
        [Authorize]
        public async Task<IActionResult> Create()
        {
            ViewBag.Categories = await _context.ForumCategories.ToListAsync();
            return View(new ForumPostViewModel());
        }

        // POST: /Forum/Create
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ForumPostViewModel model)
        {
            if (ModelState.IsValid)
            {
                var post = new ForumPost
                {
                    Title = model.Title,
                    Content = model.Content,
                    CategoryId = model.CategoryId,
                    CreatedAt = DateTime.Now,
                    UserId = User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                };
                _context.ForumPosts.Add(post);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Categories = await _context.ForumCategories.ToListAsync();
            return View(model);
        }

        // POST: /Forum/CreateComment
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateComment(int postId, string content, string quoteContent)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                return RedirectToAction("Details", new { id = postId });
            }

            var post = await _context.ForumPosts
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.Id == postId);

            if (post == null)
            {
                return NotFound();
            }

            var comment = new ForumComment
            {
                Content = content,
                CreatedAt = DateTime.Now,
                UserId = User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.NameIdentifier)?.Value,
                ForumPostId = postId,
                QuoteContent = string.IsNullOrWhiteSpace(quoteContent) ? null : quoteContent
            };
            _context.ForumComments.Add(comment);

            // Tạo thông báo cho chủ bài viết nếu không phải là người comment
            if (post.UserId != comment.UserId)
            {
                var notification = new ForumNotification
                {
                    UserId = post.UserId,
                    Message = $"Có người đã trả lời bài viết của bạn: {post.Title}",
                    Type = "Reply",
                    ForumPostId = postId,
                    ForumCommentId = comment.Id,
                    CreatedAt = DateTime.Now
                };
                _context.ForumNotifications.Add(notification);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Details", new { id = postId });
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> UploadImage(IFormFile upload)
        {
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
            if (upload != null && upload.Length > 0)
            {
                var ext = Path.GetExtension(upload.FileName).ToLowerInvariant();
                if (!allowedExtensions.Contains(ext))
                {
                    return Json(new { error = new { message = "Chỉ cho phép upload ảnh định dạng jpg, jpeg, png, gif, webp." } });
                }
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "forum");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);
                var fileName = Guid.NewGuid().ToString() + ext;
                var filePath = Path.Combine(uploadsFolder, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await upload.CopyToAsync(stream);
                }
                var url = "/uploads/forum/" + fileName;
                return Json(new { url });
            }
            return Json(new { error = new { message = "Upload failed. File không hợp lệ hoặc rỗng." } });
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReportPost(int postId, string reason)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(reason) || string.IsNullOrWhiteSpace(userId))
                return Json(new { success = false, message = "Lý do báo cáo không được để trống." });
            var report = new ForumReport
            {
                ForumPostId = postId,
                UserId = userId,
                Reason = reason,
                CreatedAt = DateTime.Now
            };
            _context.ForumReports.Add(report);
            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Đã gửi báo cáo. Cảm ơn bạn!" });
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReportComment(int commentId, string reason)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(reason) || string.IsNullOrWhiteSpace(userId))
                return Json(new { success = false, message = "Lý do báo cáo không được để trống." });
            var report = new ForumReport
            {
                ForumCommentId = commentId,
                UserId = userId,
                Reason = reason,
                CreatedAt = DateTime.Now
            };
            _context.ForumReports.Add(report);
            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Đã gửi báo cáo. Cảm ơn bạn!" });
        }
    }
} 