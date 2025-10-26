using MovizoneApp.Models;

namespace MovizoneApp.Core.Interfaces
{
    public interface IJwtService
    {
        string GenerateToken(User user);
        int? ValidateToken(string token);
    }
}
