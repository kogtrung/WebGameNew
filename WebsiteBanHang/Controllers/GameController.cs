using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebGame.Models;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using System.Diagnostics;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using WebGame.Services;
using System;

using System.Text;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using WebGame.Data;

namespace WebGame.Controllers
{
    public class GameController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<GameController> _logger;
        private readonly IGameImageService _gameImageService;
        private readonly UserManager<ApplicationUser> _userManager;

        // Danh sách đường dẫn ảnh cục bộ (sẽ được thay thế bằng dịch vụ ảnh)
        private readonly string[] sampleImages = new string[]
        {
            "https://image.api.playstation.com/vulcan/ap/rnd/202108/0410/0Imhn60XcUSstCELZQE7FTxW.png", // Elden Ring
            "https://image.api.playstation.com/vulcan/ap/rnd/202207/1210/4xJ8XB3bi888QTLZYdl7Oi0s.png", // God of War
            "https://image.api.playstation.com/vulcan/ap/rnd/202010/0222/niMUubpU9y1PxNvYmDfb8QFD.png", // Last of Us 2
            "https://image.api.playstation.com/vulcan/ap/rnd/202202/2816/mYn2ETBKFcg0DlLcOc5yd0iu.png", // GTA V
            "https://image.api.playstation.com/vulcan/ap/rnd/202111/3013/cJdbb8URYlKgxcRVmgCFfB9w.png", // Cyberpunk 2077
            "https://image.api.playstation.com/vulcan/ap/rnd/202107/3100/HO8vkO9pfXhwbHi5WHECQJdN.png", // Horizon Forbidden West
            "https://assets.nintendo.com/image/upload/c_fill,w_1200/q_auto:best/f_auto/dpr_2.0/ncom/software/switch/70010000001130/c42553b4fd0312c31e70ec7468c6c9bccd739f340152925b9600631f2d29f8b5", // Super Mario Odyssey
            "https://image.api.playstation.com/vulcan/ap/rnd/202010/0915/RYWdAMWqbTNkjQ4AliLuGj8R.png", // Uncharted 4
            "https://image.api.playstation.com/vulcan/ap/rnd/202010/0222/p50N0QDQDDIWsVvKZNLRn99X.png"  // Ghost of Tsushima
        };

        public GameController(ApplicationDbContext context, ILogger<GameController> logger, IGameImageService gameImageService, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _logger = logger;
            _gameImageService = gameImageService;
            _userManager = userManager;
        }

        // GET: /Game
        public async Task<IActionResult> Index(string platform = null, string genre = null, int? year = null, string platforms = null, string genres = null, int page = 1)
        {
            try
            {
                // Khởi tạo danh sách trò chơi
                List<Game> games = await _context.Games
                    .Include(g => g.GamePlatforms)
                    .ThenInclude(gp => gp.Platform)
                    .ToListAsync();
                
                // Chuyển đổi mảng trò chơi thành danh sách có thể lọc
                var filteredGames = games.ToList();
                
                // Lọc theo nền tảng từ tham số cũ (single platform)
                if (!string.IsNullOrEmpty(platform))
                {
                    // Chuẩn hóa tên nền tảng để xử lý các trường hợp khác nhau
                    var normalizedPlatform = platform;
                    
                    // Xác định Xbox Series vs Xbox Series X
                    if (platform.Contains("Xbox Series") && !platform.Contains("Xbox Series X"))
                    {
                        normalizedPlatform = "Xbox Series X";
                    }
                    
                    filteredGames = filteredGames.Where(g => 
                        // Kiểm tra trong GamePlatforms
                        (g.GamePlatforms != null && 
                            g.GamePlatforms.Any(gp => 
                                gp.Platform != null && 
                                (gp.Platform.Name.Contains(normalizedPlatform) || 
                                    normalizedPlatform.Contains(gp.Platform.Name))
                            ))
                        ||
                        // Hoặc kiểm tra trong thuộc tính Platform
                        (g.Platform != null && 
                            (g.Platform.Contains(normalizedPlatform) || 
                            // Kiểm tra nếu Platform chứa nền tảng dưới dạng danh sách phân tách bằng dấu phẩy
                            g.Platform.Split(',').Select(p => p.Trim()).Any(p => 
                                p.Contains(normalizedPlatform) || normalizedPlatform.Contains(p))))
                    ).ToList();
                }
                
                // Lọc theo danh sách nền tảng mới (multiple platforms)
                if (!string.IsNullOrEmpty(platforms))
                {
                    var platformList = platforms.Split(',').Select(p => p.Trim()).ToList();
                    
                    if (platformList.Any())
                    {
                        // Create a new filtered list that matches any of the selected platforms
                        filteredGames = filteredGames.Where(g => 
                            // Check in GamePlatforms
                            (g.GamePlatforms != null && 
                                g.GamePlatforms.Any(gp => 
                                    gp.Platform != null && 
                                    platformList.Any(p => 
                                        gp.Platform.Name.Contains(p) || 
                                        p.Contains(gp.Platform.Name))
                                ))
                            ||
                            // Or check in Platform property
                            (g.Platform != null && 
                                platformList.Any(p => 
                                    g.Platform.Contains(p) || 
                                    g.Platform.Split(',').Select(pl => pl.Trim())
                                        .Any(pl => pl.Contains(p) || p.Contains(pl))
                                )
                            )
                        ).ToList();
                        
                        _logger.LogInformation($"Found {filteredGames.Count} games for platforms: {platforms}");
                    }
                }

                // Apply genre filter (old parameter)
                if (!string.IsNullOrEmpty(genre))
                {
                    filteredGames = filteredGames.Where(g => 
                        g.Genre != null && g.Genre.ToLower().Contains(genre.ToLower())
                    ).ToList();
                }
                
                // Apply genres filter (new parameter - multiple genres)
                if (!string.IsNullOrEmpty(genres))
                {
                    var selectedGenreList = genres.Split(',').Select(g => g.Trim().ToLower()).ToList();
                    
                    if (selectedGenreList.Any())
                    {
                        filteredGames = filteredGames.Where(g => 
                            g.Genre != null && 
                            selectedGenreList.Any(selectedGenre => 
                                g.Genre.ToLower().Contains(selectedGenre) ||
                                g.Genre.ToLower().Split(',').Select(g => g.Trim())
                                    .Any(g => g.Contains(selectedGenre) || selectedGenre.Contains(g))
                            )
                        ).ToList();
                        
                        _logger.LogInformation($"Found {filteredGames.Count} games for genres: {genres}");
                    }
                }
                
                // Apply year filter
                if (year.HasValue)
                {
                    filteredGames = filteredGames.Where(g => 
                        g.ReleaseDate.HasValue && g.ReleaseDate.Value.Year == year.Value
                    ).ToList();
                    
                    _logger.LogInformation($"Found {filteredGames.Count} games for year {year}");
                }

                // Nếu sau khi lọc không có game nào, sử dụng sample games
                if (!filteredGames.Any())
                {
                    _logger.LogInformation("No games found after filtering, using sample games");
                    var sampleGames = CreateSampleGames();
                    
                    // Lọc sample games dựa trên các tiêu chí đã chọn
                if (!string.IsNullOrEmpty(platform))
                {
                        sampleGames = sampleGames.Where(g => 
                            g.Platform != null && g.Platform.Contains(platform)
                        ).ToList();
                    }
                    
                    if (!string.IsNullOrEmpty(platforms))
                    {
                        var platformList = platforms.Split(',').Select(p => p.Trim()).ToList();
                        sampleGames = sampleGames.Where(g => 
                            g.Platform != null && 
                            platformList.Any(p => g.Platform.Contains(p))
                        ).ToList();
                    }
                    
                if (!string.IsNullOrEmpty(genre))
                {
                        sampleGames = sampleGames.Where(g => 
                            g.Genre != null && g.Genre.ToLower().Contains(genre.ToLower())
                        ).ToList();
                    }
                    
                    if (!string.IsNullOrEmpty(genres))
                    {
                        var selectedGenreList = genres.Split(',').Select(g => g.Trim().ToLower()).ToList();
                        sampleGames = sampleGames.Where(g => 
                            g.Genre != null && 
                            selectedGenreList.Any(selectedGenre => g.Genre.ToLower().Contains(selectedGenre))
                        ).ToList();
                    }
                    
                    if (year.HasValue)
                    {
                        sampleGames = sampleGames.Where(g => 
                            g.ReleaseDate.HasValue && g.ReleaseDate.Value.Year == year.Value
                        ).ToList();
                    }
                    
                    filteredGames = sampleGames;
                    _logger.LogInformation($"Added {filteredGames.Count} sample games matching criteria");
                }

                // Calculate pagination
                int pageSize = 12;
                var totalItems = filteredGames.Count;
                var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

                // Ensure page number is valid
                page = Math.Max(1, Math.Min(page, Math.Max(1, totalPages)));

                // Get games for current page
                var pagedGames = filteredGames
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                // Ensure all games have images
                AssignImages(pagedGames);
                
                // Ensure all games have platform information
                AssignPlatforms(pagedGames);

                // Get unique genres for filter dropdown
                var genreList = new List<string>();
                foreach (var game in games)
                {
                    if (!string.IsNullOrEmpty(game.Genre))
                    {
                        var genreParts = game.Genre.Split(',');
                        foreach (var part in genreParts)
                        {
                            var trimmed = part.Trim();
                            if (!string.IsNullOrEmpty(trimmed) && !genreList.Contains(trimmed))
                            {
                                genreList.Add(trimmed);
                            }
                        }
                    }
                }
                genreList.Sort();

                // Set up ViewBag values
                ViewBag.Genres = genreList;
                ViewBag.CurrentGenre = genre;
                ViewBag.CurrentPlatform = platform;
                ViewBag.CurrentYear = year;
                ViewBag.CurrentPage = page;
                ViewBag.TotalPages = totalPages;

                return View(pagedGames);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Index action");
                return View("Error", new ErrorViewModel
                {
                    RequestId = HttpContext.TraceIdentifier,
                    Message = "An error occurred while loading the games. Please try again later."
                });
            }
        }

        private List<Game> CreateSampleGames()
        {
            // Generate unique IDs for sample games that are negative to avoid conflicts with database
            return new List<Game>
            {
                new Game
                {
                    Id = -1,
                    Title = "God of War Ragnarök",
                    Description = "God of War Ragnarök is an action-adventure game developed by Santa Monica Studio and published by Sony Interactive Entertainment. It was released worldwide on November 9, 2022, for the PlayStation 4 and PlayStation 5, marking the first cross-gen release in the God of War series.",
                    ImageUrl = "https://image.api.playstation.com/vulcan/ap/rnd/202207/0706/CQh4aqIWJSwvRGWKIQDwNMBG.jpg",
                    VideoUrl = "https://www.youtube.com/watch?v=EE-4GvjKcfs",
                    TrailerUrl = "https://www.youtube.com/watch?v=EE-4GvjKcfs",
                    MetaScore = 94,
                    ReleaseDate = new DateTime(2022, 11, 9),
                    Platform = "PlayStation 5, PlayStation 4",
                    Genre = "Action, Adventure",
                    Developer = "Santa Monica Studio",
                    Publisher = "Sony Interactive Entertainment",
                    Rating = "M"
                },
                new Game
                {
                    Id = -2,
                    Title = "The Last of Us Part II",
                    Description = "The Last of Us Part II is an action-adventure game developed by Naughty Dog and published by Sony Interactive Entertainment. Set five years after The Last of Us (2013), the game focuses on two playable characters in a post-apocalyptic United States whose lives intertwine: Ellie, who sets out for revenge after suffering a tragedy, and Abby, a soldier who becomes involved in a conflict between her militia and a religious cult.",
                    ImageUrl = "https://image.api.playstation.com/vulcan/ap/rnd/202010/0222/niMUubpU9y1PxNvYmDfb8QFD.png",
                    VideoUrl = "https://www.youtube.com/watch?v=qPNiIeKMHyg",
                    TrailerUrl = "https://www.youtube.com/watch?v=qPNiIeKMHyg",
                    MetaScore = 93,
                    ReleaseDate = new DateTime(2020, 6, 19),
                    Platform = "PlayStation 5, PlayStation 4",
                    Genre = "Action, Adventure",
                    Developer = "Naughty Dog",
                    Publisher = "Sony Interactive Entertainment",
                    Rating = "M"
                },
                new Game
                {
                    Id = -3,
                    Title = "Horizon Forbidden West",
                    Description = "Horizon Forbidden West is a 2022 action role-playing game developed by Guerrilla Games and published by Sony Interactive Entertainment. The sequel to 2017's Horizon Zero Dawn, the game is set in a post-apocalyptic version of the western United States recovering from the aftermath of an extinction event caused by a rogue robot swarm.",
                    ImageUrl = "https://image.api.playstation.com/vulcan/ap/rnd/202107/3100/HO8vkO9pfXhwbHi5WHECQJdN.png",
                    VideoUrl = "https://www.youtube.com/watch?v=SZbpSIQNzxU",
                    TrailerUrl = "https://www.youtube.com/watch?v=SZbpSIQNzxU",
                    MetaScore = 88,
                    ReleaseDate = new DateTime(2022, 2, 18),
                    Platform = "PlayStation 5, PlayStation 4",
                    Genre = "Action RPG",
                    Developer = "Guerrilla Games",
                    Publisher = "Sony Interactive Entertainment",
                    Rating = "T"
                },
                new Game
                {
                    Id = -4,
                    Title = "Ghost of Tsushima",
                    Description = "Ghost of Tsushima is a 2020 action-adventure game developed by Sucker Punch Productions and published by Sony Interactive Entertainment. The game follows Jin Sakai, a samurai on a quest to protect Tsushima Island during the first Mongol invasion of Japan.",
                    ImageUrl = "https://image.api.playstation.com/vulcan/img/rnd/202010/2618/GemRaOZaCMhGxXXkYtIlHbJ3.png",
                    VideoUrl = "https://www.youtube.com/watch?v=rTNfgIAi3pY",
                    TrailerUrl = "https://www.youtube.com/watch?v=rTNfgIAi3pY",
                    MetaScore = 83,
                    ReleaseDate = new DateTime(2020, 7, 17),
                    Platform = "PlayStation 5, PlayStation 4",
                    Genre = "Action, Adventure",
                    Developer = "Sucker Punch Productions",
                    Publisher = "Sony Interactive Entertainment",
                    Rating = "M"
                },
                new Game
                {
                    Id = -5,
                    Title = "Marvel's Spider-Man 2",
                    Description = "Marvel's Spider-Man 2 is a 2023 action-adventure game developed by Insomniac Games and published by Sony Interactive Entertainment. Based on Marvel Comics' Spider-Man character, it is the sequel to Marvel's Spider-Man (2018) and Marvel's Spider-Man: Miles Morales (2020).",
                    ImageUrl = "https://image.api.playstation.com/vulcan/ap/rnd/202306/1219/1c7afc402e2e8969378d2d26d48767989tw2bm3o.jpg",
                    VideoUrl = "https://www.youtube.com/watch?v=9fVYKsEmuRo",
                    TrailerUrl = "https://www.youtube.com/watch?v=9fVYKsEmuRo",
                    MetaScore = 90,
                    ReleaseDate = new DateTime(2023, 10, 20),
                    Platform = "PlayStation 5",
                    Genre = "Action, Adventure",
                    Developer = "Insomniac Games",
                    Publisher = "Sony Interactive Entertainment",
                    Rating = "T"
                },
                new Game
                {
                    Id = -6,
                    Title = "Final Fantasy XVI",
                    Description = "Final Fantasy XVI is an action role-playing game developed and published by Square Enix. The sixteenth main installment in the Final Fantasy series, it was released for the PlayStation 5 on June 22, 2023.",
                    ImageUrl = "https://image.api.playstation.com/vulcan/ap/rnd/202211/0819/S1jCzktHlS9JIYnlZ787Q2Kh.jpg",
                    VideoUrl = "https://www.youtube.com/watch?v=yr6PtdY0i7M",
                    TrailerUrl = "https://www.youtube.com/watch?v=yr6PtdY0i7M",
                    MetaScore = 87,
                    ReleaseDate = new DateTime(2023, 6, 22),
                    Platform = "PlayStation 5",
                    Genre = "Action RPG",
                    Developer = "Square Enix",
                    Publisher = "Square Enix",
                    Rating = "M"
                },
                new Game
                {
                    Id = -7,
                    Title = "Ratchet & Clank: Rift Apart",
                    Description = "Ratchet & Clank: Rift Apart is a 2021 third-person shooter platform game developed by Insomniac Games and published by Sony Interactive Entertainment for the PlayStation 5.",
                    ImageUrl = "https://image.api.playstation.com/vulcan/ap/rnd/202101/2921/OcZ4eq2L7aKIWxlZXLl68JZE.jpg",
                    VideoUrl = "https://www.youtube.com/watch?v=9p_gg9UW9k4",
                    TrailerUrl = "https://www.youtube.com/watch?v=9p_gg9UW9k4",
                    MetaScore = 88,
                    ReleaseDate = new DateTime(2021, 6, 11),
                    Platform = "PlayStation 5",
                    Genre = "Platform, Action",
                    Developer = "Insomniac Games",
                    Publisher = "Sony Interactive Entertainment",
                    Rating = "E10+"
                },
                new Game
                {
                    Id = -8,
                    Title = "Demon's Souls",
                    Description = "Demon's Souls is a 2020 action role-playing game developed by Bluepoint Games and published by Sony Interactive Entertainment for the PlayStation 5. It is a remake of Demon's Souls, originally developed by FromSoftware and released for the PlayStation 3 in 2009.",
                    ImageUrl = "https://image.api.playstation.com/vulcan/ap/rnd/202011/0402/Xvt1i8DFy23EMURhQx8PWFyH.jpg",
                    VideoUrl = "https://www.youtube.com/watch?v=2TMs2E6cms4",
                    TrailerUrl = "https://www.youtube.com/watch?v=2TMs2E6cms4",
                    MetaScore = 92,
                    ReleaseDate = new DateTime(2020, 11, 12),
                    Platform = "PlayStation 5",
                    Genre = "Action RPG",
                    Developer = "Bluepoint Games",
                    Publisher = "Sony Interactive Entertainment",
                    Rating = "M"
                },
                new Game
                {
                    Id = -9,
                    Title = "Returnal",
                    Description = "Returnal is a 2021 third-person shooter roguelike video game developed by Housemarque and published by Sony Interactive Entertainment for the PlayStation 5.",
                    ImageUrl = "https://image.api.playstation.com/vulcan/ap/rnd/202101/2921/22B2LQQfkCpUJQmHfK0QS7ga.png",
                    VideoUrl = "https://www.youtube.com/watch?v=Jv4BjWoB-NA",
                    TrailerUrl = "https://www.youtube.com/watch?v=Jv4BjWoB-NA",
                    MetaScore = 86,
                    ReleaseDate = new DateTime(2021, 4, 30),
                    Platform = "PlayStation 5",
                    Genre = "Third-person shooter, Roguelike",
                    Developer = "Housemarque",
                    Publisher = "Sony Interactive Entertainment",
                    Rating = "T"
                },
                new Game
                {
                    Id = -10,
                    Title = "Gran Turismo 7",
                    Description = "Gran Turismo 7 is a 2022 simulation racing video game developed by Polyphony Digital and published by Sony Interactive Entertainment. The game is the eighth mainline installment in the Gran Turismo series.",
                    ImageUrl = "https://image.api.playstation.com/vulcan/ap/rnd/202109/1323/OZvO9e1nbmrw9YZLaR0WzUSZ.jpg",
                    VideoUrl = "https://www.youtube.com/watch?v=1tBUsXIkG1A",
                    TrailerUrl = "https://www.youtube.com/watch?v=1tBUsXIkG1A",
                    MetaScore = 87,
                    ReleaseDate = new DateTime(2022, 3, 4),
                    Platform = "PlayStation 5, PlayStation 4",
                    Genre = "Racing, Simulation",
                    Developer = "Polyphony Digital",
                    Publisher = "Sony Interactive Entertainment",
                    Rating = "E"
                }
            };
        }

        private void AssignImages(List<Game> games)
        {
            foreach (var game in games)
            {
                if (string.IsNullOrEmpty(game.ImageUrl))
                {
                    // Nếu không có ảnh, đặt ảnh placeholder
                    game.ImageUrl = "/images/game-placeholder.jpg";
                }
                else if (game.ImageUrl.StartsWith("/images/"))
                {
                    // Nếu là đường dẫn nội bộ và không tồn tại, đổi về placeholder
                    string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", game.ImageUrl.TrimStart('/'));
                    if (!System.IO.File.Exists(filePath))
                    {
                        _logger.LogWarning($"Image not found at {filePath} for game {game.Title}. Using placeholder.");
                        game.ImageUrl = "/images/game-placeholder.jpg";
                    }
                }
                else if (!game.ImageUrl.StartsWith("http"))
                {
                    // Đảm bảo đường dẫn tương đối bắt đầu bằng / 
                    if (!game.ImageUrl.StartsWith("/"))
                    {
                        game.ImageUrl = "/" + game.ImageUrl;
                    }
                }
                
                // Đảm bảo có ảnh dự phòng nếu ảnh gốc không tải được
                if (!game.ImageUrl.StartsWith("/images/game-placeholder.jpg"))
                {
                    // Không cần thay đổi gì, ảnh sẽ sử dụng thuộc tính onerror trong HTML để load ảnh dự phòng nếu cần
                }
            }
            
            // Ensure platform information is assigned
            AssignPlatforms(games);
        }
        
        private void AssignPlatforms(List<Game> games)
        {
            var platformsByGenre = new Dictionary<string, string>
            {
                { "Action", "PC, PlayStation 5, Xbox Series X" },
                { "Adventure", "PC, PlayStation 5, Xbox Series X, Nintendo Switch" },
                { "RPG", "PC, PlayStation 5, Xbox Series X" },
                { "Strategy", "PC, Nintendo Switch" },
                { "Simulation", "PC, PlayStation 5" },
                { "Sports", "PC, PlayStation 5, Xbox Series X, Nintendo Switch" },
                { "Racing", "PC, PlayStation 5, Xbox Series X" },
                { "Fighting", "PlayStation 5, Xbox Series X, Nintendo Switch" },
                { "Shooter", "PC, PlayStation 5, Xbox Series X" },
                { "Platformer", "PC, PlayStation 5, Xbox Series X, Nintendo Switch" },
                { "Puzzle", "PC, PlayStation 5, Xbox Series X, Nintendo Switch, Mobile" }
            };
            
            // Dictionary for specific game titles
            var platformsByTitle = new Dictionary<string, string>
            {
                { "Final Fantasy", "PlayStation 5, PC" },
                { "Stellar Blade", "PlayStation 5" },
                { "Zelda", "Nintendo Switch" },
                { "Mario", "Nintendo Switch" },
                { "Halo", "Xbox Series X, PC" },
                { "Forza", "Xbox Series X, PC" },
                { "God of War", "PlayStation 5, PC" },
                { "Spider-Man", "PlayStation 5" },
                { "Helldivers", "PlayStation 5, PC" },
                { "Starfield", "Xbox Series X, PC" },
                { "Elden Ring", "PC, PlayStation 5, Xbox Series X" },
                { "Tekken", "PC, PlayStation 5, Xbox Series X" },
                { "Call of Duty", "PC, PlayStation 5, Xbox Series X" },
                { "Battlefield", "PC, PlayStation 5, Xbox Series X" },
                { "Assassin's Creed", "PC, PlayStation 5, Xbox Series X" },
                { "Alan Wake", "PC, PlayStation 5, Xbox Series X" },
                { "Resident Evil", "PC, PlayStation 5, Xbox Series X" },
                { "Black Myth", "PC, PlayStation 5, Xbox Series X" }
            };
            
            foreach (var game in games)
            {
                // Check if GamePlatforms collection is populated and contains valid data
                if (game.GamePlatforms != null && game.GamePlatforms.Any(gp => gp.Platform != null))
                {
                    // If we have valid GamePlatforms, update the Platform string property
                    // for consistency and for views that rely on it
                    var platformNames = game.GamePlatforms
                        .Where(gp => gp.Platform != null)
                        .Select(gp => gp.Platform.Name)
                        .OrderBy(name => name)
                        .ToList();
                    
                    if (platformNames.Any())
                    {
                        game.Platform = string.Join(", ", platformNames);
                        continue; // Skip the rest of the loop since we have proper platform data
                    }
                }
                
                // If GamePlatforms is empty but Platform property has data, check if it's valid
                if (!string.IsNullOrEmpty(game.Platform) && game.Platform != "Not specified")
                {
                    // Just make sure platform names are consistent
                    game.Platform = game.Platform.Replace("PS5", "PlayStation 5")
                                                .Replace("PS4", "PlayStation 4")
                                                .Replace("XBOX", "Xbox")
                                                .Replace("Xbox Series X/S", "Xbox Series X");
                                                
                    // Thêm các nền tảng thiếu nếu có thể 
                    // Nếu game là title phổ biến, gán đầy đủ nền tảng cho nó
                    foreach (var titleKey in platformsByTitle.Keys)
                    {
                        if (game.Title != null && game.Title.Contains(titleKey))
                        {
                            game.Platform = platformsByTitle[titleKey];
                            break;
                        }
                    }
                    
                    continue; // Skip the rest of the loop since we have platform data
                }
                
                // If we've reached here, we need to assign platforms
                string platforms = "PC, PlayStation 5, Xbox Series X"; // Default platforms
                
                // First check if the title matches any known game series
                bool titleMatch = false;
                foreach (var title in platformsByTitle.Keys)
                {
                    if (game.Title != null && game.Title.ToLower().Contains(title.ToLower()))
                    {
                        platforms = platformsByTitle[title];
                        titleMatch = true;
                        break;
                    }
                }
                
                // If no title match, try to match by genre
                if (!titleMatch && !string.IsNullOrEmpty(game.Genre))
                {
                    foreach (var genre in platformsByGenre.Keys)
                    {
                        if (game.Genre.ToLower().Contains(genre.ToLower()))
                        {
                            platforms = platformsByGenre[genre];
                            break;
                        }
                    }
                }
                
                game.Platform = platforms;
                
                // Thêm log để theo dõi
                _logger.LogInformation($"Assigned platforms for game '{game.Title}': {game.Platform}");
            }
        }

        // GET: Game/Details/5
        [HttpGet]
        [Route("Game/Details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var game = await _context.Games
                .Include(g => g.Reviews)
                .Include(g => g.GamePlatforms)
                .ThenInclude(gp => gp.Platform)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (game == null)
            {
                return NotFound();
            }

            // Check if the current user follows this game
            bool isFollowing = false;
            if (User.Identity.IsAuthenticated)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user != null)
                {
                    isFollowing = await _context.GameFollows
                        .AnyAsync(gf => gf.UserId == user.Id && gf.GameId == id);
                }
            }
            ViewBag.IsFollowing = isFollowing;

            return View(game);
        }

        // ... existing code ...
    }
} 