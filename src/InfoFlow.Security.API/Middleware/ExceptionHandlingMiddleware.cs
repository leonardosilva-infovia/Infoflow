using System;
using System.Collections.Generic;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace InfoFlow.Security.API.Middleware;

public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task Invoke(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            await HandleAsync(context, ex, logger);
        }
    }

    private static async Task HandleAsync(HttpContext context, Exception ex, ILogger logger)
    {
        // Mapeamento de exceções -> status code
        var (status, title) = ex switch
        {
            UnauthorizedAccessException => (HttpStatusCode.Unauthorized, "Unauthorized"),
            KeyNotFoundException        => (HttpStatusCode.NotFound,     "Not Found"),
            ValidationException         => (HttpStatusCode.BadRequest,   "Validation Failure"),
            InvalidOperationException   => (HttpStatusCode.BadRequest,   "Invalid Operation"),
            ArgumentException           => (HttpStatusCode.BadRequest,   "Invalid Argument"),
            _                           => (HttpStatusCode.InternalServerError, "Unexpected Error")
        };

        // Log rico (sem vazar dados sensíveis)
        if ((int)status >= 500)
            logger.LogError(ex, "Unhandled exception");
        else
            logger.LogWarning(ex, "Handled domain/application exception (mapped to {Status})", status);

        // Problem Details (RFC 7807)
        var problem = new
        {
            type   = $"https://httpstatuses.io/{(int)status}",
            title,
            status = (int)status,
            detail = ex.Message,
            traceId = context.TraceIdentifier
        };

        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = (int)status;

        var json = JsonSerializer.Serialize(problem, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
        await context.Response.WriteAsync(json);
    }
}