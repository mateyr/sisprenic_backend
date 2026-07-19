using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Sisprenic.Api.Common.ErrorHandling;

public sealed class GlobalExceptionHandler(
    IProblemDetailsService problemDetailsService,
    ILogger<GlobalExceptionHandler> logger,
    IHostEnvironment environment) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is OperationCanceledException && cancellationToken.IsCancellationRequested)
        {
            return false;
        }

        (int statusCode, string title, string detail) = MapException(exception);

        if (statusCode >= StatusCodes.Status500InternalServerError)
        {
            logger.LogError(exception, "Unhandled exception processing {Method} {Path}",
                httpContext.Request.Method,
                httpContext.Request.Path);
        }
        else
        {
            logger.LogWarning(exception, "Handled exception processing {Method} {Path}",
                httpContext.Request.Method,
                httpContext.Request.Path);
        }

        httpContext.Response.StatusCode = statusCode;

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = environment.IsDevelopment() ? exception.Message : detail,
            Instance = httpContext.Request.Path
        };

        if (environment.IsDevelopment() && statusCode >= StatusCodes.Status500InternalServerError)
        {
            problemDetails.Extensions["exception"] = exception.GetType().Name;
            problemDetails.Extensions["stackTrace"] = exception.StackTrace;
        }

        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            ProblemDetails = problemDetails,
            Exception = exception
        });
    }

    private static (int StatusCode, string Title, string Detail) MapException(Exception exception) =>
        exception switch
        {
            BadHttpRequestException => (
                StatusCodes.Status400BadRequest,
                "Solicitud inválida",
                "La solicitud no pudo ser procesada."),

            ArgumentException => (
                StatusCodes.Status400BadRequest,
                "Solicitud inválida",
                "La solicitud contiene datos inválidos."),

            UnauthorizedAccessException => (
                StatusCodes.Status403Forbidden,
                "Prohibido",
                "No tiene permisos para realizar esta acción."),

            KeyNotFoundException => (
                StatusCodes.Status404NotFound,
                "No encontrado",
                "El recurso solicitado no fue encontrado."),

            DbUpdateConcurrencyException => (
                StatusCodes.Status409Conflict,
                "Conflicto",
                "El recurso fue modificado por otro usuario."),

            DbUpdateException => (
                StatusCodes.Status409Conflict,
                "Conflicto",
                "No se pudo completar la operación debido a un conflicto con los datos existentes."),

            _ => (
                StatusCodes.Status500InternalServerError,
                "Error interno",
                "Ocurrió un error inesperado. Intente nuevamente más tarde.")
        };
}
