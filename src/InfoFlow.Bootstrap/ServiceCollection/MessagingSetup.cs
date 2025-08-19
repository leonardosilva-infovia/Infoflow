using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace InfoFlow.Bootstrap.ServiceCollection;

public static class MessagingSetup
{
  public static IServiceCollection AddInfoFlowRabbitMq(this IServiceCollection services, IConfiguration config)
  {
    var host = config["RabbitMQ:Host"];
    var user = config["RabbitMQ:Username"];
    var pass = config["RabbitMQ:Password"];

    services.AddMassTransit(x =>
    {
      x.UsingRabbitMq((ctx, cfg) =>
      {
        cfg.Host(host, h =>
        {
          if (!string.IsNullOrWhiteSpace(user)) h.Username(user);
          if (!string.IsNullOrWhiteSpace(pass)) h.Password(pass);
        });
      });
    });

    return services;
  }
}