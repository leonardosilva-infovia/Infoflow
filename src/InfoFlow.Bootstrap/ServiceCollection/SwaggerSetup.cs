using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace InfoFlow.Bootstrap.ServiceCollection;

public static class SwaggerSetup
{
  public static IServiceCollection AddInfoFlowSwagger(this IServiceCollection services, string title)
  {
    services.AddEndpointsApiExplorer();
    services.AddSwaggerGen(c =>
    {
      c.SwaggerDoc("v1", new OpenApiInfo { Title = title, Version = "v1" });
      c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
      {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Bearer. Ex: Bearer {token}"
      });
      c.AddSecurityRequirement(new OpenApiSecurityRequirement
      {
        {
          new OpenApiSecurityScheme {
            Reference = new OpenApiReference{ Type = ReferenceType.SecurityScheme, Id = "Bearer" }
          },
          Array.Empty<string>()
        }
      });
    });
    return services;
  }
}