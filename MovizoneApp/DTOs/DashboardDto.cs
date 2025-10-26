using System;
using System.Collections.Generic;

namespace MovizoneApp.DTOs
{
    /// <summary>
    /// DTO for Dashboard Statistics
    /// </summary>
    public class DashboardStatisticsDto
    {
        // Overall statistics
        public int TotalMovies { get; set; }
        public int TotalSeries { get; set; }
        public int TotalUsers { get; set; }
        public int TotalActors { get; set; }
        public int TotalComments { get; set; }
        public int TotalReviews { get; set; }

        // Monthly statistics with change tracking
        public int SubscriptionsThisMonth { get; set; }
        public int SubscriptionsChange { get; set; } // +/- number
        public int ItemsAddedThisMonth { get; set; }
        public int ItemsAddedChange { get; set; }
        public int ViewsThisMonth { get; set; }
        public double ViewsChangePercent { get; set; }
        public int ReviewsThisMonth { get; set; }
        public int ReviewsChange { get; set; }

        // Additional statistics
        public int TodayViews { get; set; }
        public decimal MonthlyRevenue { get; set; }

        // Dashboard widget data
        public List<TopItemDto> TopItems { get; set; } = new List<TopItemDto>();
        public List<LatestItemDto> LatestItems { get; set; } = new List<LatestItemDto>();
        public List<LatestUserDto> LatestUsers { get; set; } = new List<LatestUserDto>();
        public List<LatestReviewDto> LatestReviews { get; set; } = new List<LatestReviewDto>();
    }

    /// <summary>
    /// DTO for Top Items widget (sorted by rating)
    /// </summary>
    public class TopItemDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty; // "Movie" or "TV Series"
        public double Rating { get; set; }
    }

    /// <summary>
    /// DTO for Latest Items widget (sorted by creation date)
    /// </summary>
    public class LatestItemDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty; // "Movie" or "TV Series"
        public double Rating { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    /// <summary>
    /// DTO for Latest Users widget
    /// </summary>
    public class LatestUserDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    /// <summary>
    /// DTO for Latest Reviews widget
    /// </summary>
    public class LatestReviewDto
    {
        public int Id { get; set; }
        public string ItemTitle { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public double Rating { get; set; }
        public int MovieId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
