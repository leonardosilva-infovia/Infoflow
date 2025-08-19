namespace InfoFlow.Shared.Security.DTOs;

public record RegisterRequest(string FullName, string Email, string Password);
public record LoginRequest(string Email, string Password);
public record TokenResponse(string AccessToken, DateTime ExpiresAt, string RefreshToken, DateTime RefreshExpiresAt);
public record RefreshRequest(string RefreshToken);
public record ChangePasswordRequest(string CurrentPassword, string NewPassword);
public record AssignRoleRequest(string RoleName);
public record UserDto(Guid Id, string Email, string? FullName, bool IsActive, IEnumerable<string> Roles);