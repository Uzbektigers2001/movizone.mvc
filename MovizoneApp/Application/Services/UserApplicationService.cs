using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
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
    /// Uses DTOs and AutoMapper for clean separation from database models
    /// </summary>
    public class UserApplicationService : IUserApplicationService
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtService _jwtService;
        private readonly IMapper _mapper;
        private readonly ILogger<UserApplicationService> _logger;

        public UserApplicationService(
            IUserRepository userRepository,
            IJwtService jwtService,
            IMapper mapper,
            ILogger<UserApplicationService> logger)
        {
            _userRepository = userRepository;
            _jwtService = jwtService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            try
            {
                _logger.LogInformation("Fetching all users");
                var users = await _userRepository.GetAllAsync();
                return _mapper.Map<IEnumerable<UserDto>>(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching all users");
                throw;
            }
        }

        public async Task<UserDto?> GetUserByIdAsync(int id)
        {
            try
            {
                _logger.LogInformation("Fetching user with ID: {UserId}", id);
                var user = await _userRepository.GetByIdAsync(id);

                if (user == null)
                {
                    _logger.LogWarning("User with ID {UserId} not found", id);
                    return null;
                }

                return _mapper.Map<UserDto>(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching user with ID: {UserId}", id);
                throw;
            }
        }

        public async Task<UserDto?> GetUserByEmailAsync(string email)
        {
            try
            {
                _logger.LogInformation("Fetching user with email: {Email}", email);
                var user = await _userRepository.GetByEmailAsync(email);
                return user == null ? null : _mapper.Map<UserDto>(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching user with email: {Email}", email);
                throw;
            }
        }

        public async Task<UserDto> CreateUserAsync(CreateUserDto createUserDto)
        {
            try
            {
                _logger.LogInformation("Creating new user: {UserEmail}", createUserDto.Email);

                // Business validation (additional to DTO validation)
                if (string.IsNullOrWhiteSpace(createUserDto.Password) || createUserDto.Password.Length < 6)
                {
                    throw new BadRequestException("Password must be at least 6 characters");
                }

                // Check if email already exists
                if (await _userRepository.EmailExistsAsync(createUserDto.Email))
                {
                    throw new BadRequestException("Email already registered");
                }

                // Map DTO to Model
                var user = _mapper.Map<User>(createUserDto);

                // Hash password and set timestamps
                user.SetPassword(createUserDto.Password);
                user.CreatedAt = DateTime.UtcNow;
                user.IsActive = true;

                // Save to repository
                var created = await _userRepository.AddAsync(user);
                _logger.LogInformation("User created successfully with ID: {UserId}", created.Id);

                // Map back to DTO and return
                return _mapper.Map<UserDto>(created);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user: {UserEmail}", createUserDto.Email);
                throw;
            }
        }

        public async Task UpdateUserAsync(UpdateUserDto updateUserDto)
        {
            try
            {
                _logger.LogInformation("Updating user with ID: {UserId}", updateUserDto.Id);

                // Check if user exists
                var existing = await _userRepository.GetByIdAsync(updateUserDto.Id);
                if (existing == null)
                {
                    throw new NotFoundException("User", updateUserDto.Id);
                }

                // Business validation
                if (string.IsNullOrWhiteSpace(updateUserDto.Email))
                {
                    throw new BadRequestException("Email is required");
                }

                // Check if email is taken by another user
                if (updateUserDto.Email != existing.Email)
                {
                    var emailExists = await _userRepository.GetByEmailAsync(updateUserDto.Email);
                    if (emailExists != null && emailExists.Id != updateUserDto.Id)
                    {
                        throw new BadRequestException("Email already registered");
                    }
                }

                // Map DTO properties to existing tracked entity
                _mapper.Map(updateUserDto, existing);

                // Set update time (CreatedAt and Password already preserved in existing entity)
                existing.UpdatedAt = DateTime.UtcNow;

                // Update in repository
                await _userRepository.UpdateAsync(existing);
                _logger.LogInformation("User updated successfully: {UserId}", existing.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user with ID: {UserId}", updateUserDto.Id);
                throw;
            }
        }

        public async Task DeleteUserAsync(int id)
        {
            try
            {
                _logger.LogInformation("Deleting user with ID: {UserId}", id);

                var user = await _userRepository.GetByIdAsync(id);
                if (user == null)
                {
                    throw new NotFoundException("User", id);
                }

                // Soft delete
                user.IsDeleted = true;
                user.DeletedAt = DateTime.UtcNow;
                // TODO: Set DeletedBy from current user context when authentication is available
                // entity.DeletedBy = currentUserId;
                user.IsActive = false;
                user.UpdatedAt = DateTime.UtcNow;
                await _userRepository.UpdateAsync(user);

                _logger.LogInformation("User soft-deleted successfully: {UserId}", id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user with ID: {UserId}", id);
                throw;
            }
        }

        public async Task<UserDto?> AuthenticateAsync(string email, string password)
        {
            try
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
                return _mapper.Map<UserDto>(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error authenticating user: {Email}", email);
                throw;
            }
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
        {
            try
            {
                _logger.LogInformation("Login attempt for email: {Email}", loginDto.Email);

                var userDto = await AuthenticateAsync(loginDto.Email, loginDto.Password);
                if (userDto == null)
                {
                    throw new UnauthorizedException("Invalid email or password");
                }

                // Get the actual User entity for JWT generation
                var userEntity = await _userRepository.GetByIdAsync(userDto.Id);
                var token = _jwtService.GenerateToken(userEntity!);

                return new AuthResponseDto
                {
                    UserId = userDto.Id,
                    Name = userDto.Name,
                    Email = userDto.Email,
                    Role = userDto.Role,
                    Token = token,
                    ExpiresAt = DateTime.UtcNow.AddHours(2)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login for email: {Email}", loginDto.Email);
                throw;
            }
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto)
        {
            try
            {
                _logger.LogInformation("Registration attempt for email: {Email}", registerDto.Email);

                var createUserDto = new CreateUserDto
                {
                    Name = registerDto.Name,
                    Email = registerDto.Email,
                    Password = registerDto.Password,
                    Role = "User"
                };

                var createdDto = await CreateUserAsync(createUserDto);

                // Get the actual User entity for JWT generation
                var userEntity = await _userRepository.GetByIdAsync(createdDto.Id);
                var token = _jwtService.GenerateToken(userEntity!);

                return new AuthResponseDto
                {
                    UserId = createdDto.Id,
                    Name = createdDto.Name,
                    Email = createdDto.Email,
                    Role = createdDto.Role,
                    Token = token,
                    ExpiresAt = DateTime.UtcNow.AddHours(2)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during registration for email: {Email}", registerDto.Email);
                throw;
            }
        }

        public async Task ChangePasswordAsync(ChangePasswordDto changePasswordDto)
        {
            try
            {
                _logger.LogInformation("Changing password for user ID: {UserId}", changePasswordDto.UserId);

                var user = await _userRepository.GetByIdAsync(changePasswordDto.UserId);
                if (user == null)
                {
                    throw new NotFoundException("User", changePasswordDto.UserId);
                }

                // Verify current password
                if (!user.VerifyPassword(changePasswordDto.CurrentPassword))
                {
                    throw new BadRequestException("Current password is incorrect");
                }

                // Validate new password
                if (string.IsNullOrWhiteSpace(changePasswordDto.NewPassword) || changePasswordDto.NewPassword.Length < 6)
                {
                    throw new BadRequestException("New password must be at least 6 characters");
                }

                // Set new password
                user.SetPassword(changePasswordDto.NewPassword);
                user.UpdatedAt = DateTime.UtcNow;

                await _userRepository.UpdateAsync(user);
                _logger.LogInformation("Password changed successfully for user ID: {UserId}", changePasswordDto.UserId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing password for user ID: {UserId}", changePasswordDto.UserId);
                throw;
            }
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            try
            {
                return await _userRepository.EmailExistsAsync(email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if email exists: {Email}", email);
                throw;
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            try
            {
                return await _userRepository.ExistsAsync(u => u.Id == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if user exists with ID: {UserId}", id);
                throw;
            }
        }
    }
}
