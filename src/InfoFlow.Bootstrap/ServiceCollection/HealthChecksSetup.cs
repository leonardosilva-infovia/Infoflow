using Microsoft.Extensions.DependencyInjection;

namespace InfoFlow.Bootstrap.ServiceCollection;

public static class HealthChecksSetup
{
  public static IServiceCollection AddInfoFlowHealthChecks(this IServiceCollection services)
  {
    services.AddHealthChecks();
    return services;
  }
}