using InfoFlow.Application.Security.Abstractions;
using InfoFlow.Infrastructure.Security;
using Microsoft.Extensions.DependencyInjection;

namespace InfoFlow.Infrastructure;

public static class DependencyInjection
{
  public static IServiceCollection AddApplicationServices(this IServiceCollection services)
  {
    services.AddScoped<ITokenService, TokenService>();
    services.AddScoped<IPermissionProvider, DefaultPermissionProvider>();
    services.AddScoped<IIdentityService, IdentityService>();
    return services;
  }
}