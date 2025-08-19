using Microsoft.AspNetCore.Builder;

namespace InfoFlow.Security.API.Middleware;

public static class MiddlewareExtensions
{
  public static IApplicationBuilder UseGlobalExceptionHandling(this IApplicationBuilder app)
    => app.UseMiddleware<ExceptionHandlingMiddleware>();
}