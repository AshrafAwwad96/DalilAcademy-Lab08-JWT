using DalilAcademy.Core.Exceptions;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace DalilAcademy.API.Middleware;

// One class catches every exception from every endpoint and turns it into the right status code.
// 500 errors are logged — and never leak details.
public class GlobalExceptionHandler(
    IProblemDetailsService problemDetails,
    ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken ct)
    {
        var (status, title) = exception switch
        {
            ValidationException => (StatusCodes.Status400BadRequest, "Validation failed"),
            NotFoundException   => (StatusCodes.Status404NotFound,   "Resource not found"),
            ConflictException   => (StatusCodes.Status409Conflict,   "Conflict"),
            UnauthorizedException => (StatusCodes.Status401Unauthorized, "Unauthorized"),
            _                   => (StatusCodes.Status500InternalServerError, "An unexpected error occurred")
        };

        if (status == StatusCodes.Status500InternalServerError)
            logger.LogError(exception, "Unhandled exception");

        context.Response.StatusCode = status;
        var problem = new ProblemDetails
        {
            Status = status,
            Title = title,
            Detail = exception is NotFoundException or ConflictException or UnauthorizedException ? exception.Message : null
        };

        if (exception is ValidationException ve)
            problem.Extensions["errors"] = ve.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

        return await problemDetails.TryWriteAsync(new() { HttpContext = context, ProblemDetails = problem });
    }
}
