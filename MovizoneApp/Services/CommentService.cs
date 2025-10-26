using System;
using System.Collections.Generic;
using System.Linq;
using MovizoneApp.Models;

namespace MovizoneApp.Services
{
    public class CommentService : ICommentService
    {
        private static List<Comment> _comments = new List<Comment>();
        private static int _nextId = 1;

        public CommentService()
        {
            // Initialize with some sample data
            if (!_comments.Any())
            {
                _comments.AddRange(new[]
                {
                    new Comment
                    {
                        Id = _nextId++,
                        MovieId = 1,
                        UserId = 1,
                        UserName = "John Doe",
                        Text = "Great movie! Really enjoyed watching it.",
                        IsApproved = true,
                        CreatedAt = DateTime.Now.AddDays(-5)
                    },
                    new Comment
                    {
                        Id = _nextId++,
                        MovieId = 1,
                        UserId = 2,
                        UserName = "Jane Smith",
                        Text = "Amazing cinematography and storytelling.",
                        IsApproved = true,
                        CreatedAt = DateTime.Now.AddDays(-3)
                    },
                    new Comment
                    {
                        Id = _nextId++,
                        MovieId = 2,
                        UserId = 3,
                        UserName = "Mike Johnson",
                        Text = "This movie is pending approval",
                        IsApproved = false,
                        CreatedAt = DateTime.Now.AddHours(-2)
                    }
                });
            }
        }

        public List<Comment> GetAllComments()
        {
            return _comments.OrderByDescending(c => c.CreatedAt).ToList();
        }

        public List<Comment> GetCommentsByMovie(int movieId)
        {
            return _comments
                .Where(c => c.MovieId == movieId && c.IsApproved)
                .OrderByDescending(c => c.CreatedAt)
                .ToList();
        }

        public List<Comment> GetCommentsByTVSeries(int tvSeriesId)
        {
            return _comments
                .Where(c => c.TVSeriesId == tvSeriesId && c.IsApproved)
                .OrderByDescending(c => c.CreatedAt)
                .ToList();
        }

        public List<Comment> GetPendingComments()
        {
            return _comments
                .Where(c => !c.IsApproved)
                .OrderByDescending(c => c.CreatedAt)
                .ToList();
        }

        public Comment? GetCommentById(int id)
        {
            return _comments.FirstOrDefault(c => c.Id == id);
        }

        public void AddComment(Comment comment)
        {
            comment.Id = _nextId++;
            comment.CreatedAt = DateTime.Now;
            _comments.Add(comment);
        }

        public void UpdateComment(Comment comment)
        {
            var existing = GetCommentById(comment.Id);
            if (existing != null)
            {
                existing.Text = comment.Text;
                existing.IsApproved = comment.IsApproved;
                existing.UpdatedAt = DateTime.Now;
            }
        }

        public void DeleteComment(int id)
        {
            var comment = GetCommentById(id);
            if (comment != null)
            {
                _comments.Remove(comment);
            }
        }

        public void ApproveComment(int id)
        {
            var comment = GetCommentById(id);
            if (comment != null)
            {
                comment.IsApproved = true;
                comment.UpdatedAt = DateTime.Now;
            }
        }

        public void RejectComment(int id)
        {
            DeleteComment(id);
        }
    }
}
