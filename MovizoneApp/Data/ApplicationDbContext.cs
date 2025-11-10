using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MovizoneApp.Models;

namespace MovizoneApp.Data
{
    /// <summary>
    /// Main database context for the application with auditing and soft delete support
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
        public DbSet<Episode> Episodes { get; set; } = null!;
        public DbSet<SiteSettings> SiteSettings { get; set; } = null!;

        // Many-to-many join entities
        public DbSet<MovieActor> MovieActors { get; set; } = null!;
        public DbSet<TVSeriesActor> TVSeriesActors { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Apply configurations
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

            // Global query filter for soft deletes - applies to ALL entities that inherit from BaseAuditableEntity
            modelBuilder.Entity<Movie>().HasQueryFilter(m => !m.IsDeleted);
            modelBuilder.Entity<TVSeries>().HasQueryFilter(t => !t.IsDeleted);
            modelBuilder.Entity<Actor>().HasQueryFilter(a => !a.IsDeleted);
            modelBuilder.Entity<User>().HasQueryFilter(u => !u.IsDeleted);
            modelBuilder.Entity<Review>().HasQueryFilter(r => !r.IsDeleted);
            modelBuilder.Entity<Episode>().HasQueryFilter(e => !e.IsDeleted);
            modelBuilder.Entity<SiteSettings>().HasQueryFilter(s => !s.IsDeleted);

            // Indexes for better performance
            modelBuilder.Entity<Movie>().HasIndex(m => m.Title);
            modelBuilder.Entity<Movie>().HasIndex(m => m.Genre);
            modelBuilder.Entity<Movie>().HasIndex(m => m.IsDeleted);
            modelBuilder.Entity<Movie>().HasIndex(m => m.CreatedAt);

            modelBuilder.Entity<TVSeries>().HasIndex(t => t.Title);
            modelBuilder.Entity<TVSeries>().HasIndex(t => t.Genre);
            modelBuilder.Entity<TVSeries>().HasIndex(t => t.IsDeleted);
            modelBuilder.Entity<TVSeries>().HasIndex(t => t.CreatedAt);

            modelBuilder.Entity<Actor>().HasIndex(a => a.Name);
            modelBuilder.Entity<Actor>().HasIndex(a => a.IsDeleted);
            modelBuilder.Entity<Actor>().HasIndex(a => a.CreatedAt);

            modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();
            modelBuilder.Entity<User>().HasIndex(u => u.IsDeleted);
            modelBuilder.Entity<User>().HasIndex(u => u.CreatedAt);

            modelBuilder.Entity<Review>().HasIndex(r => r.MovieId);
            modelBuilder.Entity<Review>().HasIndex(r => r.UserId);
            modelBuilder.Entity<Review>().HasIndex(r => r.IsDeleted);

            modelBuilder.Entity<Episode>().HasIndex(e => e.TVSeriesId);
            modelBuilder.Entity<Episode>().HasIndex(e => e.IsDeleted);

            // Configure MovieActor many-to-many relationship
            modelBuilder.Entity<MovieActor>()
                .HasKey(ma => new { ma.MovieId, ma.ActorId }); // Composite primary key

            modelBuilder.Entity<MovieActor>()
                .HasOne(ma => ma.Movie)
                .WithMany(m => m.MovieActors)
                .HasForeignKey(ma => ma.MovieId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<MovieActor>()
                .HasOne(ma => ma.Actor)
                .WithMany(a => a.MovieActors)
                .HasForeignKey(ma => ma.ActorId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<MovieActor>().HasIndex(ma => ma.DisplayOrder);

            // Configure TVSeriesActor many-to-many relationship
            modelBuilder.Entity<TVSeriesActor>()
                .HasKey(tsa => new { tsa.TVSeriesId, tsa.ActorId }); // Composite primary key

            modelBuilder.Entity<TVSeriesActor>()
                .HasOne(tsa => tsa.TVSeries)
                .WithMany(ts => ts.TVSeriesActors)
                .HasForeignKey(tsa => tsa.TVSeriesId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TVSeriesActor>()
                .HasOne(tsa => tsa.Actor)
                .WithMany(a => a.TVSeriesActors)
                .HasForeignKey(tsa => tsa.ActorId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TVSeriesActor>().HasIndex(tsa => tsa.DisplayOrder);

            // Configure Review to support both Movie and TVSeries
            // Review must have either MovieId OR TVSeriesId, not both
            modelBuilder.Entity<Review>()
                .HasIndex(r => r.TVSeriesId); // Add index for TVSeriesId

            modelBuilder.Entity<Review>()
                .HasOne(r => r.Movie)
                .WithMany()
                .HasForeignKey(r => r.MovieId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired(false); // Optional relationship

            modelBuilder.Entity<Review>()
                .HasOne(r => r.TVSeries)
                .WithMany()
                .HasForeignKey(r => r.TVSeriesId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired(false); // Optional relationship
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            // Automatically set audit timestamps for all BaseAuditableEntity entities
            var entries = ChangeTracker.Entries()
                .Where(e => e.Entity is Core.Models.BaseAuditableEntity &&
                           (e.State == EntityState.Added || e.State == EntityState.Modified));

            foreach (var entry in entries)
            {
                var entity = (Core.Models.BaseAuditableEntity)entry.Entity;
                var now = DateTime.UtcNow;

                if (entry.State == EntityState.Added)
                {
                    // Set creation timestamp if not already set
                    if (entity.CreatedAt == default || entity.CreatedAt == DateTime.MinValue)
                    {
                        entity.CreatedAt = now;
                    }
                    // Note: CreatedBy should be set by the service layer from the current user context
                    // We don't set it here because DbContext doesn't have access to HTTP context
                }
                else if (entry.State == EntityState.Modified)
                {
                    // Always update the modification timestamp
                    entity.UpdatedAt = now;
                    // Note: UpdatedBy should be set by the service layer from the current user context

                    // Prevent modification of creation audit fields
                    entry.Property(nameof(Core.Models.BaseAuditableEntity.CreatedAt)).IsModified = false;
                    entry.Property(nameof(Core.Models.BaseAuditableEntity.CreatedBy)).IsModified = false;
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
