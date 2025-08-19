using InfoFlow.Application.Security.Abstractions;
using InfoFlow.Persistence.Services;
using Microsoft.Extensions.DependencyInjection;

namespace InfoFlow.Persistence;

public static class DependencyInjection
{
  public static IServiceCollection AddPersistenceServices(this IServiceCollection services)
  {
    services.AddScoped<IRefreshTokenService, EfRefreshTokenService>();
    return services;
  }
}