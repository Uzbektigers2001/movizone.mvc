using System.Collections.Generic;
using MovizoneApp.Models;

namespace MovizoneApp.Services
{
    public interface ICommentService
    {
        List<Comment> GetAllComments();
        List<Comment> GetCommentsByMovie(int movieId);
        List<Comment> GetCommentsByTVSeries(int tvSeriesId);
        List<Comment> GetPendingComments();
        Comment? GetCommentById(int id);
        void AddComment(Comment comment);
        void UpdateComment(Comment comment);
        void DeleteComment(int id);
        void ApproveComment(int id);
        void RejectComment(int id);
    }
}
