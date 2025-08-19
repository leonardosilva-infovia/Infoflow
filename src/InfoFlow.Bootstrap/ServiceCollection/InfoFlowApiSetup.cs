using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace InfoFlow.Bootstrap.ServiceCollection;

public static class InfoFlowApiSetup
{
  /// <summary>
  /// Registra o conjunto padrão de serviços para uma API InfoFlow (exceto Identity).
  /// </summary>
  public static IServiceCollection AddInfoFlowApiCore(
    this IServiceCollection services,
    IConfiguration config,
    IHostEnvironment env,
    string serviceName,
    IEnumerable<string> permissionsForPolicies)
  {
    services
      .AddInfoFlowSerilog(config, env, serviceName)
      .AddInfoFlowApiVersioning()
      .AddInfoFlowSwagger($"{serviceName}")
      .AddInfoFlowCors(config)
      .AddInfoFlowJwtAuthentication(config)
      .AddInfoFlowPermissionPolicies(permissionsForPolicies)
      .AddInfoFlowHttpClients()
      .AddInfoFlowRabbitMq(config)
      .AddInfoFlowHealthChecks()
      .AddInfoFlowControllers();

    return services;
  }
}