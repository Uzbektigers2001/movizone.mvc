using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MovizoneApp.Core.Exceptions;
using MovizoneApp.Core.Interfaces;
using MovizoneApp.DTOs;
using MovizoneApp.Models;

namespace MovizoneApp.Controllers
{
    /// <summary>
    /// API Controller for authentication using JWT
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AuthApiController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtService _jwtService;
        private readonly ILogger<AuthApiController> _logger;

        public AuthApiController(
            IUserRepository userRepository,
            IJwtService jwtService,
            ILogger<AuthApiController> logger)
        {
            _userRepository = userRepository;
            _jwtService = jwtService;
            _logger = logger;
        }

        /// <summary>
        /// Login endpoint
        /// POST: api/authapi/login
        /// </summary>
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginDto loginDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _logger.LogInformation("Login attempt for email: {Email}", loginDto.Email);

            var user = await _userRepository.GetByEmailAsync(loginDto.Email);
            if (user == null || !user.VerifyPassword(loginDto.Password))
            {
                _logger.LogWarning("Failed login attempt for email: {Email}", loginDto.Email);
                throw new UnauthorizedException("Invalid email or password");
            }

            if (!user.IsActive)
            {
                _logger.LogWarning("Login attempt for inactive user: {Email}", loginDto.Email);
                throw new UnauthorizedException("Account is inactive");
            }

            var token = _jwtService.GenerateToken(user);

            _logger.LogInformation("User {Email} logged in successfully", loginDto.Email);

            return Ok(new AuthResponseDto
            {
                UserId = user.Id,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role,
                Token = token,
                ExpiresAt = DateTime.UtcNow.AddHours(2)
            });
        }

        /// <summary>
        /// Register endpoint
        /// POST: api/authapi/register
        /// </summary>
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<ActionResult<AuthResponseDto>> Register([FromBody] RegisterDto registerDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _logger.LogInformation("Registration attempt for email: {Email}", registerDto.Email);

            // Check if email already exists
            if (await _userRepository.EmailExistsAsync(registerDto.Email))
            {
                _logger.LogWarning("Registration failed - email already exists: {Email}", registerDto.Email);
                throw new BadRequestException("Email already registered");
            }

            // Create new user
            var user = new User
            {
                Name = registerDto.Name,
                Email = registerDto.Email,
                Role = "User",
                IsActive = true,
                Avatar = "/img/user.svg"
            };

            user.SetPassword(registerDto.Password);

            await _userRepository.AddAsync(user);

            var token = _jwtService.GenerateToken(user);

            _logger.LogInformation("User {Email} registered successfully", registerDto.Email);

            return Ok(new AuthResponseDto
            {
                UserId = user.Id,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role,
                Token = token,
                ExpiresAt = DateTime.UtcNow.AddHours(2)
            });
        }

        /// <summary>
        /// Get current user info (requires authentication)
        /// GET: api/authapi/me
        /// </summary>
        [HttpGet("me")]
        [Authorize]
        public async Task<ActionResult<User>> GetCurrentUser()
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                throw new UnauthorizedException("User not authenticated");
            }

            var userId = int.Parse(userIdClaim.Value);
            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null)
            {
                throw new NotFoundException("User", userId);
            }

            // Don't return password
            user.Password = string.Empty;

            return Ok(user);
        }
    }
}
