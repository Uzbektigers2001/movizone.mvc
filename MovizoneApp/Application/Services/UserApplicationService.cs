using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MovizoneApp.Application.Interfaces;
using MovizoneApp.Core.Exceptions;
using MovizoneApp.Core.Interfaces;
using MovizoneApp.DTOs;
using MovizoneApp.Models;

namespace MovizoneApp.Application.Services
{
    /// <summary>
    /// Application service for user business logic
    /// </summary>
    public class UserApplicationService : IUserApplicationService
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtService _jwtService;
        private readonly ILogger<UserApplicationService> _logger;

        public UserApplicationService(
            IUserRepository userRepository,
            IJwtService jwtService,
            ILogger<UserApplicationService> logger)
        {
            _userRepository = userRepository;
            _jwtService = jwtService;
            _logger = logger;
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            _logger.LogInformation("Fetching all users");
            var users = await _userRepository.GetAllAsync();

            return users.ToList();
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            _logger.LogInformation("Fetching user with ID: {UserId}", id);
            var user = await _userRepository.GetByIdAsync(id);

            if (user == null)
            {
                _logger.LogWarning("User with ID {UserId} not found", id);
            }

            return user;
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            _logger.LogInformation("Fetching user with email: {Email}", email);
            return await _userRepository.GetByEmailAsync(email);
        }

        public async Task<User> CreateUserAsync(User user, string plainPassword)
        {
            _logger.LogInformation("Creating new user: {UserEmail}", user.Email);

            // Business validation
            if (string.IsNullOrWhiteSpace(user.Email))
            {
                throw new BadRequestException("Email is required");
            }

            if (string.IsNullOrWhiteSpace(user.Name))
            {
                throw new BadRequestException("Name is required");
            }

            if (string.IsNullOrWhiteSpace(plainPassword) || plainPassword.Length < 6)
            {
                throw new BadRequestException("Password must be at least 6 characters");
            }

            // Check if email already exists
            if (await _userRepository.EmailExistsAsync(user.Email))
            {
                throw new BadRequestException("Email already registered");
            }

            user.SetPassword(plainPassword);
            user.CreatedAt = DateTime.UtcNow;
            user.IsActive = true;

            var created = await _userRepository.AddAsync(user);
            _logger.LogInformation("User created successfully with ID: {UserId}", created.Id);

            return created;
        }

        public async Task UpdateUserAsync(User user)
        {
            _logger.LogInformation("Updating user with ID: {UserId}", user.Id);

            var existing = await _userRepository.GetByIdAsync(user.Id);
            if (existing == null)
            {
                throw new NotFoundException("User", user.Id);
            }

            // Business validation
            if (string.IsNullOrWhiteSpace(user.Email))
            {
                throw new BadRequestException("Email is required");
            }

            // Check if email is taken by another user
            if (user.Email != existing.Email)
            {
                var emailExists = await _userRepository.GetByEmailAsync(user.Email);
                if (emailExists != null && emailExists.Id != user.Id)
                {
                    throw new BadRequestException("Email already registered");
                }
            }

            user.UpdatedAt = DateTime.UtcNow;
            user.CreatedAt = existing.CreatedAt;
            user.Password = existing.Password; // Don't update password here

            await _userRepository.UpdateAsync(user);
            _logger.LogInformation("User updated successfully: {UserId}", user.Id);
        }

        public async Task DeleteUserAsync(int id)
        {
            _logger.LogInformation("Deleting user with ID: {UserId}", id);

            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                throw new NotFoundException("User", id);
            }

            // Soft delete
            user.IsDeleted = true;
            user.IsActive = false;
            user.UpdatedAt = DateTime.UtcNow;
            await _userRepository.UpdateAsync(user);

            _logger.LogInformation("User soft-deleted successfully: {UserId}", id);
        }

        public async Task<User?> AuthenticateAsync(string email, string password)
        {
            _logger.LogInformation("Authenticating user: {Email}", email);

            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null)
            {
                _logger.LogWarning("Authentication failed - user not found: {Email}", email);
                return null;
            }

            if (!user.VerifyPassword(password))
            {
                _logger.LogWarning("Authentication failed - invalid password for: {Email}", email);
                return null;
            }

            if (!user.IsActive)
            {
                _logger.LogWarning("Authentication failed - user inactive: {Email}", email);
                return null;
            }

            _logger.LogInformation("User authenticated successfully: {Email}", email);
            return user;
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
        {
            _logger.LogInformation("Login attempt for email: {Email}", loginDto.Email);

            var user = await AuthenticateAsync(loginDto.Email, loginDto.Password);
            if (user == null)
            {
                throw new UnauthorizedException("Invalid email or password");
            }

            var token = _jwtService.GenerateToken(user);

            return new AuthResponseDto
            {
                UserId = user.Id,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role,
                Token = token,
                ExpiresAt = DateTime.UtcNow.AddHours(2)
            };
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto)
        {
            _logger.LogInformation("Registration attempt for email: {Email}", registerDto.Email);

            var user = new User
            {
                Name = registerDto.Name,
                Email = registerDto.Email,
                Role = "User",
                IsActive = true,
                Avatar = "/img/user.svg"
            };

            var created = await CreateUserAsync(user, registerDto.Password);
            var token = _jwtService.GenerateToken(created);

            return new AuthResponseDto
            {
                UserId = created.Id,
                Name = created.Name,
                Email = created.Email,
                Role = created.Role,
                Token = token,
                ExpiresAt = DateTime.UtcNow.AddHours(2)
            };
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _userRepository.EmailExistsAsync(email);
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _userRepository.ExistsAsync(u => u.Id == id);
        }
    }
}
