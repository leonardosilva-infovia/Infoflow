using InfoFlow.Application.Security.Abstractions;
using InfoFlow.Domain.Security.Entities;
using InfoFlow.Shared.Security.Authorization;

namespace InfoFlow.Infrastructure.Security;

/// <summary>
/// Implementação simples baseada em mapeamento estático de roles para permissões.
/// Em produção, você pode migrar isso para banco de dados (tabelas role_permissions).
/// </summary>
public class DefaultPermissionProvider : IPermissionProvider
{
  public Task<IReadOnlyCollection<string>> GetPermissionsAsync(AppUser user, IEnumerable<string> roles, CancellationToken ct = default)
  {
    var set = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

    // Admin tem tudo
    if (roles.Any(r => string.Equals(r, "admin", StringComparison.OrdinalIgnoreCase)))
    {
      foreach (var p in Permissions.All) set.Add(p);
      return Task.FromResult<IReadOnlyCollection<string>>(set.ToArray());
    }

    // Exemplos de papéis intermediários
    if (roles.Any(r => string.Equals(r, "manager", StringComparison.OrdinalIgnoreCase)))
    {
      set.Add(Permissions.SecurityUsersRead);
      set.Add(Permissions.SecurityRolesRead);
      set.Add(Permissions.SecurityUsersManage);
    }

    if (roles.Any(r => string.Equals(r, "reader", StringComparison.OrdinalIgnoreCase)))
    {
      set.Add(Permissions.SecurityUsersRead);
      set.Add(Permissions.SecurityRolesRead);
    }

    return Task.FromResult<IReadOnlyCollection<string>>(set.ToArray());
  }
}