using System;

namespace MovizoneApp.Models
{
    public class Review
    {
        public int Id { get; set; }
        public int MovieId { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Comment { get; set; } = string.Empty;
        public int Rating { get; set; } // 1-10
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
