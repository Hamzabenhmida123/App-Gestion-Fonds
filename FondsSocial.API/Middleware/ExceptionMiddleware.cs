using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FondsSocial.API.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (DbUpdateException ex) when (IsUniqueConstraintViolation(ex))
            {
                _logger.LogWarning(ex, "Violation de contrainte unique sur {Method} {Path}",
                    httpContext.Request.Method, httpContext.Request.Path);
                await HandleConflictAsync(httpContext);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception non gérée sur {Method} {Path}",
                    httpContext.Request.Method, httpContext.Request.Path);
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        // SQL Server error 2601: duplicate key row (unique index); 2627: violation of unique constraint/PK.
        private static bool IsUniqueConstraintViolation(DbUpdateException ex)
        {
            return ex.InnerException is SqlException sqlEx && (sqlEx.Number == 2601 || sqlEx.Number == 2627);
        }

        private static Task HandleConflictAsync(HttpContext context)
        {
            var result = JsonSerializer.Serialize(new { error = "Une valeur unique existe déjà (doublon)." });
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.Conflict;
            return context.Response.WriteAsync(result);
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var code = HttpStatusCode.InternalServerError;
            var result = JsonSerializer.Serialize(new { error = "An unexpected error occurred." });
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;
            return context.Response.WriteAsync(result);
        }
    }
}
