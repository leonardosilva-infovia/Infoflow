namespace InfoFlow.Shared.Security.DTOs;

public sealed record RoleDto(Guid Id, string Name);
public sealed record CreateRoleRequest(string Name);
public sealed record AddRoleToUserRequest(Guid UserId, string RoleName);
public sealed record RemoveRoleFromUserRequest(Guid UserId, string RoleName);