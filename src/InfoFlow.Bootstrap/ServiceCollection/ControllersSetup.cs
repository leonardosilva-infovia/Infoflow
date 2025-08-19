using Microsoft.Extensions.DependencyInjection;

namespace InfoFlow.Bootstrap.ServiceCollection;

public static class ControllersSetup
{
  public static IServiceCollection AddInfoFlowControllers(this IServiceCollection services)
  {
    services.AddControllers()
      .ConfigureApiBehaviorOptions(o =>
      {
        // Config padrão para evitar ModelState automático, se quiser customizar
        // o.SuppressModelStateInvalidFilter = true;
      });

    return services;
  }
}