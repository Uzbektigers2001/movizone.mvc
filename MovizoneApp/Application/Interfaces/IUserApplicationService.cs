using System.Collections.Generic;
using System.Threading.Tasks;
using MovizoneApp.DTOs;

namespace MovizoneApp.Application.Interfaces
{
    /// <summary>
    /// Application service for user-related business logic
    /// Now uses DTOs instead of exposing database models
    /// </summary>
    public interface IUserApplicationService
    {
        Task<IEnumerable<UserDto>> GetAllUsersAsync();
        Task<UserDto?> GetUserByIdAsync(int id);
        Task<UserDto?> GetUserByEmailAsync(string email);
        Task<UserDto> CreateUserAsync(CreateUserDto createUserDto);
        Task UpdateUserAsync(UpdateUserDto updateUserDto);
        Task DeleteUserAsync(int id);
        Task<UserDto?> AuthenticateAsync(string email, string password);
        Task<AuthResponseDto> LoginAsync(LoginDto loginDto);
        Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto);
        Task ChangePasswordAsync(ChangePasswordDto changePasswordDto);
        Task<bool> EmailExistsAsync(string email);
        Task<bool> ExistsAsync(int id);
    }
}
