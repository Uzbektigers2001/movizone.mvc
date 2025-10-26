using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MovizoneApp.Application.Interfaces;
using MovizoneApp.Core.Exceptions;
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
        private readonly IUserApplicationService _userService;
        private readonly ILogger<AuthApiController> _logger;

        public AuthApiController(
            IUserApplicationService userService,
            ILogger<AuthApiController> logger)
        {
            _userService = userService;
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

            var response = await _userService.LoginAsync(loginDto);
            return Ok(response);
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

            var response = await _userService.RegisterAsync(registerDto);
            return Ok(response);
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
            var user = await _userService.GetUserByIdAsync(userId);

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
