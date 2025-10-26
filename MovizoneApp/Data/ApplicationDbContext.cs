using System;
using Microsoft.EntityFrameworkCore;
using MovizoneApp.Models;

namespace MovizoneApp.Data
{
    /// <summary>
    /// Main database context for the application
    /// </summary>
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Movie> Movies { get; set; } = null!;
        public DbSet<TVSeries> TVSeries { get; set; } = null!;
        public DbSet<Actor> Actors { get; set; } = null!;
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Review> Reviews { get; set; } = null!;
        public DbSet<WatchlistItem> WatchlistItems { get; set; } = null!;
        public DbSet<PricingPlan> PricingPlans { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Apply configurations
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

            // Global query filter for soft deletes
            modelBuilder.Entity<Movie>().HasQueryFilter(m => !m.IsDeleted);
            modelBuilder.Entity<TVSeries>().HasQueryFilter(t => !t.IsDeleted);
            modelBuilder.Entity<Actor>().HasQueryFilter(a => !a.IsDeleted);
            modelBuilder.Entity<User>().HasQueryFilter(u => !u.IsDeleted);

            // Indexes for better performance
            modelBuilder.Entity<Movie>().HasIndex(m => m.Title);
            modelBuilder.Entity<Movie>().HasIndex(m => m.Genre);
            modelBuilder.Entity<TVSeries>().HasIndex(t => t.Title);
            modelBuilder.Entity<TVSeries>().HasIndex(t => t.Genre);
            modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();
            modelBuilder.Entity<Review>().HasIndex(r => r.MovieId);
            modelBuilder.Entity<WatchlistItem>().HasIndex(w => w.UserId);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            // Automatically set timestamps
            var entries = ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Added)
                {
                    if (entry.Entity is Movie movie)
                        movie.CreatedAt = DateTime.UtcNow;
                    if (entry.Entity is TVSeries series)
                        series.CreatedAt = DateTime.UtcNow;
                    if (entry.Entity is Actor actor)
                        actor.CreatedAt = DateTime.UtcNow;
                    if (entry.Entity is User user && user.CreatedAt == default)
                        user.CreatedAt = DateTime.UtcNow;
                }
                else if (entry.State == EntityState.Modified)
                {
                    if (entry.Entity is Movie movie)
                        movie.UpdatedAt = DateTime.UtcNow;
                    if (entry.Entity is TVSeries series)
                        series.UpdatedAt = DateTime.UtcNow;
                    if (entry.Entity is Actor actor)
                        actor.UpdatedAt = DateTime.UtcNow;
                    if (entry.Entity is User user)
                        user.UpdatedAt = DateTime.UtcNow;
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
