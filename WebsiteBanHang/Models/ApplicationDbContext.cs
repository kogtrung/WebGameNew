namespace WebGame.Models
{
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;
    using WebGame.Models;
    using WebGame.Data;
    using System;

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) 
            : base(options)
        {
            // Set recommended performance options
            this.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            this.ChangeTracker.AutoDetectChangesEnabled = false;
        }
        
        public DbSet<GameCategory> GameCategories { get; set; }
        public DbSet<NewsPost> NewsPosts { get; set; }
        public DbSet<Game> Games { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Platform> Platforms { get; set; }
        public DbSet<GamePlatform> GamePlatforms { get; set; }
        
        // New DbSets for enhanced features
        public DbSet<GameFollow> GameFollows { get; set; }
        public DbSet<GameNotification> GameNotifications { get; set; }
       

        public DbSet<ForumCategory> ForumCategories { get; set; }
        public DbSet<ForumPost> ForumPosts { get; set; }
        public DbSet<ForumComment> ForumComments { get; set; }
        public DbSet<ForumPostVote> ForumPostVotes { get; set; }
        public DbSet<ForumCommentVote> ForumCommentVotes { get; set; }
        public DbSet<ForumReport> ForumReports { get; set; }
        public DbSet<ForumNotification> ForumNotifications { get; set; }

        public DbSet<Achievement> Achievements { get; set; }
        public DbSet<UserAchievement> UserAchievements { get; set; }
        public DbSet<LeaderboardEntry> LeaderboardEntries { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

          
                
            // NewsPosts
            modelBuilder.Entity<NewsPost>()
                .HasIndex(n => n.CreatedAt);
                
            modelBuilder.Entity<NewsPost>()
                .HasIndex(n => n.GameCategoryId);
                
            modelBuilder.Entity<NewsPost>()
                .HasIndex(n => n.Title);
                
            // Games
            modelBuilder.Entity<Game>()
                .HasIndex(g => g.Title);
                
            modelBuilder.Entity<Game>()
                .HasIndex(g => g.ReleaseDate);
                
            // Reviews
            modelBuilder.Entity<Review>()
                .HasIndex(r => r.GameId);
                
            modelBuilder.Entity<Review>()
                .HasIndex(r => r.UserId);
                
            // Configure relationships
            modelBuilder.Entity<NewsPost>()
                .HasOne(n => n.GameCategory)
                .WithMany()
                .HasForeignKey(n => n.GameCategoryId)
                .OnDelete(DeleteBehavior.SetNull);
                
            
                
            modelBuilder.Entity<Review>()
                .HasOne(r => r.Game)
                .WithMany(g => g.Reviews)
                .HasForeignKey(r => r.GameId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure GamePlatform relationships
            modelBuilder.Entity<GamePlatform>()
                .HasOne(gp => gp.Game)
                .WithMany(g => g.GamePlatforms)
                .HasForeignKey(gp => gp.GameId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<GamePlatform>()
                .HasOne(gp => gp.Platform)
                .WithMany(p => p.GamePlatforms)
                .HasForeignKey(gp => gp.PlatformId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure relationships and constraints
            modelBuilder.Entity<GameFollow>()
                .HasOne(gf => gf.User)
                .WithMany()
                .HasForeignKey(gf => gf.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            
            modelBuilder.Entity<GameFollow>()
                .HasOne(gf => gf.Game)
                .WithMany(g => g.Followers)
                .HasForeignKey(gf => gf.GameId)
                .OnDelete(DeleteBehavior.Cascade);
            
            modelBuilder.Entity<GameNotification>()
                .HasOne(gn => gn.User)
                .WithMany()
                .HasForeignKey(gn => gn.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            
            modelBuilder.Entity<GameNotification>()
                .HasOne(gn => gn.Game)
                .WithMany(g => g.Notifications)
                .HasForeignKey(gn => gn.GameId)
                .OnDelete(DeleteBehavior.Cascade);
            
           
            
            
            
           
            
            

            // Seed data in correct order
            // 1. Seed GameCategories first (required by NewsPost)
            GameCategorySeeder.SeedGameCategories(modelBuilder);
            
            // 2. Seed Platforms (required by GamePlatforms)
            PlatformSeeder.SeedPlatforms(modelBuilder);
            
            // 3. Seed Games
            GameSeeder.SeedGames(modelBuilder);
            
            // 4. Seed GamePlatforms (requires both Games and Platforms)
            GamePlatformSeeder.SeedGamePlatforms(modelBuilder);

            modelBuilder.Entity<ForumComment>()
                .HasOne(fc => fc.ForumPost)
                .WithMany(fp => fp.Comments)
                .HasForeignKey(fc => fc.ForumPostId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ForumCommentVote>()
                .HasOne(v => v.ForumComment)
                .WithMany(c => c.Votes)
                .HasForeignKey(v => v.ForumCommentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ForumPostVote>()
                .HasOne(v => v.ForumPost)
                .WithMany(p => p.Votes)
                .HasForeignKey(v => v.ForumPostId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ForumReport>()
                .HasOne(r => r.ForumPost)
                .WithMany()
                .HasForeignKey(r => r.ForumPostId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ForumReport>()
                .HasOne(r => r.ForumComment)
                .WithMany()
                .HasForeignKey(r => r.ForumCommentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Achievement>()
                .HasOne(a => a.Game)
                .WithMany(g => g.Achievements)
                .HasForeignKey(a => a.GameId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}