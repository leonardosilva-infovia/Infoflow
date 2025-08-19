using Microsoft.Extensions.DependencyInjection;

namespace InfoFlow.Bootstrap.ServiceCollection;

public static class AuthorizationSetup
{
  /// <summary>
  /// Adiciona policies simples baseadas em claims "perm".
  /// Passe a lista completa de permissões que a API exige (ex.: Permissions.All).
  /// </summary>
  public static IServiceCollection AddInfoFlowPermissionPolicies(this IServiceCollection services, IEnumerable<string> permissions)
  {
    services.AddAuthorization(opt =>
    {
      foreach (var p in permissions)
        opt.AddPolicy(p, pol => pol.RequireClaim("perm", p));
    });
    return services;
  }
}