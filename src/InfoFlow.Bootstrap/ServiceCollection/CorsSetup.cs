using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace InfoFlow.Bootstrap.ServiceCollection;

public static class CorsSetup
{
  public static IServiceCollection AddInfoFlowCors(this IServiceCollection services, IConfiguration config)
  {
    var origins = (config["Cors:Origins"] ?? "*")
      .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

    services.AddCors(opt =>
    {
      opt.AddPolicy("default", p =>
      {
        if (origins.Length == 0 || (origins.Length == 1 && origins[0] == "*"))
          p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
        else
          p.WithOrigins(origins).AllowAnyHeader().AllowAnyMethod();
      });
    });

    return services;
  }
}