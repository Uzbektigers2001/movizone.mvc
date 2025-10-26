using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MovizoneApp.Core.Exceptions;

namespace MovizoneApp.Middleware
{
    /// <summary>
    /// Global error handling middleware
    /// </summary>
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;

        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred: {Message}", ex.Message);
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var response = context.Response;
            response.ContentType = "application/json";

            var errorResponse = new ErrorResponse
            {
                Message = exception.Message,
                StatusCode = (int)HttpStatusCode.InternalServerError
            };

            switch (exception)
            {
                case NotFoundException:
                    errorResponse.StatusCode = (int)HttpStatusCode.NotFound;
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    break;

                case BadRequestException:
                    errorResponse.StatusCode = (int)HttpStatusCode.BadRequest;
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    break;

                case UnauthorizedException:
                    errorResponse.StatusCode = (int)HttpStatusCode.Unauthorized;
                    response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    break;

                case ForbiddenException:
                    errorResponse.StatusCode = (int)HttpStatusCode.Forbidden;
                    response.StatusCode = (int)HttpStatusCode.Forbidden;
                    break;

                case ValidationException validationEx:
                    errorResponse.StatusCode = (int)HttpStatusCode.BadRequest;
                    errorResponse.Errors = validationEx.Errors;
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    break;

                default:
                    errorResponse.StatusCode = (int)HttpStatusCode.InternalServerError;
                    response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    errorResponse.Message = "An internal server error occurred.";
                    break;
            }

            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            var result = JsonSerializer.Serialize(errorResponse, jsonOptions);
            await response.WriteAsync(result);
        }

        private class ErrorResponse
        {
            public string Message { get; set; } = string.Empty;
            public int StatusCode { get; set; }
            public IDictionary<string, string[]>? Errors { get; set; }
        }
    }
}
