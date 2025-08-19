using InfoFlow.Domain.Security.Entities;

namespace InfoFlow.Application.Security.Abstractions;

/// <summary>
/// Resolve permissões efetivas de um usuário (normalmente a partir das roles).
/// </summary>
public interface IPermissionProvider
{
  Task<IReadOnlyCollection<string>> GetPermissionsAsync(AppUser user, IEnumerable<string> roles,
    CancellationToken ct = default);
}