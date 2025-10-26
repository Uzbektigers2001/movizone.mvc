using System;
using System.Collections.Generic;

namespace MovizoneApp.Models
{
    public class DashboardStatistics
    {
        public int TotalMovies { get; set; }
        public int TotalSeries { get; set; }
        public int TotalUsers { get; set; }
        public int TotalActors { get; set; }
        public int TodayViews { get; set; }
        public int MonthlyViews { get; set; }
        public decimal MonthlyRevenue { get; set; }
        public List<RecentActivity> RecentActivities { get; set; } = new List<RecentActivity>();
    }

    public class RecentActivity
    {
        public string Type { get; set; } = string.Empty; // "movie_added", "user_registered", etc.
        public string Description { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public string Icon { get; set; } = string.Empty;
    }
}
