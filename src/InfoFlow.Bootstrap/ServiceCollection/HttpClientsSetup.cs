using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;

namespace InfoFlow.Bootstrap.ServiceCollection;

public static class HttpClientsSetup
{
  public static IServiceCollection AddInfoFlowHttpClients(this IServiceCollection services)
  {
    var retry = HttpPolicyExtensions
      .HandleTransientHttpError()
      .WaitAndRetryAsync([TimeSpan.FromMilliseconds(200), TimeSpan.FromMilliseconds(500), TimeSpan.FromSeconds(1)]);

    services.AddHttpClient("default").AddPolicyHandler(retry);
    return services;
  }
}