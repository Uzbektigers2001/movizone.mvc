using System;
using System.Collections.Generic;

namespace MovizoneApp.Models
{
    public class DashboardStatistics
    {
        // Main statistics
        public int TotalMovies { get; set; }
        public int TotalSeries { get; set; }
        public int TotalUsers { get; set; }
        public int TotalActors { get; set; }
        public int TotalComments { get; set; }
        public int TotalReviews { get; set; }

        // Monthly statistics
        public int SubscriptionsThisMonth { get; set; }
        public int SubscriptionsChange { get; set; } // +/- number
        public int ItemsAddedThisMonth { get; set; }
        public int ItemsAddedChange { get; set; } // +/- number
        public int ViewsThisMonth { get; set; }
        public double ViewsChangePercent { get; set; } // +/- percentage
        public int ReviewsThisMonth { get; set; }
        public int ReviewsChange { get; set; } // +/- number

        // Today statistics
        public int TodayViews { get; set; }
        public decimal MonthlyRevenue { get; set; }

        // Top items
        public List<TopItem> TopItems { get; set; } = new List<TopItem>();

        // Latest items
        public List<LatestItem> LatestItems { get; set; } = new List<LatestItem>();

        // Latest users
        public List<LatestUser> LatestUsers { get; set; } = new List<LatestUser>();

        // Latest reviews
        public List<LatestReview> LatestReviews { get; set; } = new List<LatestReview>();

        // Recent activities (for future use)
        public List<RecentActivity> RecentActivities { get; set; } = new List<RecentActivity>();
    }

    public class TopItem
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty; // "Movie" or "TV Series"
        public double Rating { get; set; }
    }

    public class LatestItem
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public double Rating { get; set; }
    }

    public class LatestUser
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
    }

    public class LatestReview
    {
        public int Id { get; set; }
        public string ItemTitle { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public double Rating { get; set; }
        public int MovieId { get; set; }
    }

    public class RecentActivity
    {
        public string Type { get; set; } = string.Empty; // "movie_added", "user_registered", etc.
        public string Description { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public string Icon { get; set; } = string.Empty;
    }
}
