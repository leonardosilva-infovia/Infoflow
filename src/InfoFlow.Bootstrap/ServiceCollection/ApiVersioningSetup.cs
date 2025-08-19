using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;

namespace InfoFlow.Bootstrap.ServiceCollection;

public static class ApiVersioningSetup
{
  public static IServiceCollection AddInfoFlowApiVersioning(this IServiceCollection services)
  {
    services.AddApiVersioning(opt =>
      {
        opt.AssumeDefaultVersionWhenUnspecified = true;
        opt.DefaultApiVersion = new ApiVersion(1, 0);
        opt.ReportApiVersions = true;
      })
      .AddApiExplorer(opt =>
      {
        opt.GroupNameFormat = "'v'VVV";
        opt.SubstituteApiVersionInUrl = true;
      });

    return services;
  }
}