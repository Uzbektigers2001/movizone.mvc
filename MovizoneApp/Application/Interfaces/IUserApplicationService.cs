using System.Collections.Generic;
using System.Threading.Tasks;
using MovizoneApp.DTOs;
using MovizoneApp.Models;

namespace MovizoneApp.Application.Interfaces
{
    /// <summary>
    /// Application service for user-related business logic
    /// </summary>
    public interface IUserApplicationService
    {
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<User?> GetUserByIdAsync(int id);
        Task<User?> GetUserByEmailAsync(string email);
        Task<User> CreateUserAsync(User user, string plainPassword);
        Task UpdateUserAsync(User user);
        Task DeleteUserAsync(int id);
        Task<User?> AuthenticateAsync(string email, string password);
        Task<AuthResponseDto> LoginAsync(LoginDto loginDto);
        Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto);
        Task<bool> EmailExistsAsync(string email);
        Task<bool> ExistsAsync(int id);
    }
}
