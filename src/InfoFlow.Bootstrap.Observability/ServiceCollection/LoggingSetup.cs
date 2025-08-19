using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace InfoFlow.Bootstrap.Observability.ServiceCollection;

public static class LoggingSetup
{
  public static ILoggingBuilder AddInfoFlowLogging(this ILoggingBuilder builder)
  {
    // Por enquanto, mantenha os providers padrões (Console/Debug) ou Serilog já configurado na API.
    return builder;
  }
}