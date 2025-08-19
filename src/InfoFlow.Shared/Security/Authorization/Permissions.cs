namespace InfoFlow.Shared.Security.Authorization;

/// <summary>
/// Catálogo central de permissões (nomes estáveis).
/// Cada string vira uma policy e uma claim "perm" no JWT.
/// </summary>
public static class Permissions
{
  // Usuários
  public const string SecurityUsersRead   = "Security.Users.Read";
  public const string SecurityUsersManage = "Security.Users.Manage";

  // Roles
  public const string SecurityRolesRead   = "Security.Roles.Read";
  public const string SecurityRolesManage = "Security.Roles.Manage";

  // Tokens (revogar, listar sessões etc.)
  public const string SecurityTokensManage = "Security.Tokens.Manage";

  /// <summary>Conjunto completo para facilitar registro de políticas.</summary>
  public static IReadOnlyCollection<string> All { get; } = new[]
  {
    SecurityUsersRead,
    SecurityUsersManage,
    SecurityRolesRead,
    SecurityRolesManage,
    SecurityTokensManage
  };
}