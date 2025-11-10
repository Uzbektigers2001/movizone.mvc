using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using MovizoneApp.Services;

namespace MovizoneApp.Controllers
{
    public class BaseController : Controller
    {
        /// <summary>
        /// Gets the current authenticated user's ID from JWT claims or session
        /// </summary>
        /// <returns>User ID if authenticated, null otherwise</returns>
        protected int? GetCurrentUserId()
        {
            // First, try to get from JWT claims
            var userIdClaim = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrEmpty(userIdClaim) && int.TryParse(userIdClaim, out int jwtUserId))
            {
                return jwtUserId;
            }

            // Fallback to session-based authentication
            var sessionUserId = HttpContext.Session.GetString(SessionKeys.UserId);
            if (!string.IsNullOrEmpty(sessionUserId) && int.TryParse(sessionUserId, out int sessionId))
            {
                return sessionId;
            }

            return null;
        }

        /// <summary>
        /// Gets the current authenticated user's ID, throws exception if not authenticated
        /// </summary>
        /// <returns>User ID</returns>
        /// <exception cref="UnauthorizedAccessException">Thrown when user is not authenticated</exception>
        protected int GetRequiredUserId()
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
            {
                throw new UnauthorizedAccessException("User is not authenticated");
            }
            return userId.Value;
        }

        /// <summary>
        /// Gets the current authenticated user's name from JWT claims or session
        /// </summary>
        /// <returns>User name if authenticated, null otherwise</returns>
        protected string? GetCurrentUserName()
        {
            // First, try to get from JWT claims
            var userNameClaim = User?.FindFirst(ClaimTypes.Name)?.Value;
            if (!string.IsNullOrEmpty(userNameClaim))
            {
                return userNameClaim;
            }

            // Fallback to session-based authentication
            return HttpContext.Session.GetString(SessionKeys.UserName);
        }

        /// <summary>
        /// Gets the current authenticated user's role from JWT claims or session
        /// </summary>
        /// <returns>User role if authenticated, null otherwise</returns>
        protected string? GetCurrentUserRole()
        {
            // First, try to get from JWT claims
            var roleClaim = User?.FindFirst(ClaimTypes.Role)?.Value;
            if (!string.IsNullOrEmpty(roleClaim))
            {
                return roleClaim;
            }

            // Fallback to session-based authentication
            return HttpContext.Session.GetString(SessionKeys.UserRole);
        }

        /// <summary>
        /// Checks if the current user is authenticated
        /// </summary>
        /// <returns>True if authenticated, false otherwise</returns>
        protected bool IsAuthenticated()
        {
            return GetCurrentUserId().HasValue;
        }
    }
}
