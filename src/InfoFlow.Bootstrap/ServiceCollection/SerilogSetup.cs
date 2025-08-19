using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace InfoFlow.Bootstrap.ServiceCollection;

public static class SerilogSetup
{
  public static IServiceCollection AddInfoFlowSerilog(this IServiceCollection services, IConfiguration config, IHostEnvironment env, string serviceName)
  {
    Log.Logger = new LoggerConfiguration()
      .ReadFrom.Configuration(config)
      .Enrich.FromLogContext()
      .Enrich.WithProperty("Service", serviceName)
      .CreateLogger();

    services.AddSerilog();
    return services;
  }
}