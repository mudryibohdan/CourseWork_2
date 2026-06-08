using System.Security.Claims;
using CourseWork.Business.Exceptions;
using CourseWork.Data.Enums;

namespace CourseWork.Api.Extensions;

public static class HttpContextExtensions
{
    public static int? GetUserId(this HttpContext context)
    {
        var claim = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(claim, out var userId) ? userId : null;
    }

    public static UserRole GetUserRole(this HttpContext context)
    {
        var roleClaim = context.User.FindFirstValue(ClaimTypes.Role);
        return Enum.TryParse<UserRole>(roleClaim, out var role) ? role : UserRole.Guest;
    }
}

public class BusinessExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public BusinessExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (BusinessException ex)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = ex switch
            {
                NotFoundException => StatusCodes.Status404NotFound,
                ValidationException => StatusCodes.Status400BadRequest,
                UnauthorizedBusinessException => StatusCodes.Status401Unauthorized,
                InsufficientAvailabilityException => StatusCodes.Status409Conflict,
                _ => StatusCodes.Status400BadRequest
            };

            await context.Response.WriteAsJsonAsync(new { message = ex.Message });
        }
    }
}
