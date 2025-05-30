﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebGame.Models;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Generic;
using WebGame.Repositories;

namespace WebGame.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class NewsPostController : Controller
    {
        private readonly INewsPostRepository _repository;
        private readonly IMemoryCache _cache;
        private readonly ApplicationDbContext _context;
        private const string GameCategoriesCacheKey = "GameCategories";
        private const string NewsPostsCacheKey = "NewsPosts";
        private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(10);

        public NewsPostController(INewsPostRepository repository, IMemoryCache memoryCache, ApplicationDbContext context)
        {
            _repository = repository;
            _cache = memoryCache;
            _context = context;
        }

        public async Task<IActionResult> Index(string search)
        {
            IEnumerable<NewsPost> posts;
            
            if (string.IsNullOrEmpty(search))
            {
                posts = await _repository.GetAllAsync();
            }
            else
            {
                posts = await _repository.SearchAsync(search);
            }

            ViewBag.Search = search;
            return View(posts);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var gameCategories = await _repository.GetAllCategoriesAsync();
            ViewBag.GameCategories = new SelectList(gameCategories, "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(NewsPost post)
        {
            if (ModelState.IsValid)
            {
                await _repository.AddAsync(post);

                // --- Gửi thông báo cho user đã follow các game thuộc GameCategory này ---
                if (post.GameCategoryId.HasValue && _context != null)
                {
                    var category = await _context.GameCategories.FindAsync(post.GameCategoryId.Value);
                    if (category != null)
                    {
                        var games = _context.Games.Where(g => g.Genre != null && g.Genre.Contains(category.Name)).ToList();
                        foreach (var game in games)
                        {
                            var followers = _context.GameFollows.Where(f => f.GameId == game.Id).ToList();
                            foreach (var follow in followers)
                            {
                                _context.GameNotifications.Add(new GameNotification
                                {
                                    GameId = game.Id,
                                    UserId = follow.UserId,
                                    Title = $"[Game] {game.Title} vừa có bản cập nhật mới!",
                                    Message = $"Game {game.Title} vừa có bản cập nhật mới: {post.Title}{(string.IsNullOrWhiteSpace(post.Summary) ? "" : $" - {post.Summary}")}",
                                    CreatedAt = DateTime.Now,
                                    IsRead = false,
                                    Type = NotificationType.Update
                                });
                            }
                        }
                        await _context.SaveChangesAsync();
                    }
                }
                // --- END gửi thông báo ---

                return RedirectToAction(nameof(Index));
            }
            
            var gameCategories = await _repository.GetAllCategoriesAsync();
            ViewBag.GameCategories = new SelectList(gameCategories, "Id", "Name", post.GameCategoryId);
            return View(post);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var post = await _repository.GetByIdAsync(id);
            if (post == null) return NotFound();
            
            var gameCategories = await _repository.GetAllCategoriesAsync();
            ViewBag.GameCategories = new SelectList(gameCategories, "Id", "Name", post.GameCategoryId);
            
            return View(post);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, NewsPost post)
        {
            if (id != post.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    // Check if post exists
                    if (!await _repository.ExistsAsync(id))
                    {
                        return NotFound();
                    }
                    
                    await _repository.UpdateAsync(post);
                    return RedirectToAction(nameof(Index));
                }
                catch (KeyNotFoundException)
                {
                    return NotFound();
                }
                catch (Exception)
                {
                    if (!await _repository.ExistsAsync(id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            
            var gameCategories = await _repository.GetAllCategoriesAsync();
            ViewBag.GameCategories = new SelectList(gameCategories, "Id", "Name", post.GameCategoryId);
            return View(post);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var post = await _repository.GetByIdAsync(id);
            if (post == null) return NotFound();
            return View(post);
        }

        [HttpPost, ActionName("DeleteConfirmed")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!await _repository.ExistsAsync(id))
            {
                return NotFound();
            }
            
            await _repository.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
