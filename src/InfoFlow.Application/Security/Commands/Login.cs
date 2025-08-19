using FluentValidation;
using InfoFlow.Application.Security.Abstractions;
using InfoFlow.Shared.Security.DTOs;
using MediatR;

namespace InfoFlow.Application.Security.Commands;

public record LoginCommand(LoginRequest Input) : IRequest<TokenResponse>;

public class LoginValidator : AbstractValidator<LoginCommand>
{
  public LoginValidator()
  {
    RuleFor(x => x.Input.Email).NotEmpty().EmailAddress();
    RuleFor(x => x.Input.Password).NotEmpty();
  }
}

public class LoginHandler(
  IIdentityService identity,
  ITokenService tokens,
  IPermissionProvider permissionProvider
) : IRequestHandler<LoginCommand, TokenResponse>
{
  public async Task<TokenResponse> Handle(LoginCommand request, CancellationToken ct)
  {
    var user = await identity.FindByEmailAsync(request.Input.Email);
    if (user is null || !user.IsActive) throw new UnauthorizedAccessException("Usuário inválido.");

    var ok = await identity.CheckPasswordAsync(user, request.Input.Password);
    if (!ok) throw new UnauthorizedAccessException("Credenciais inválidas.");

    var roles = await identity.GetUserRolesAsync(user);
    var permissions = await permissionProvider.GetPermissionsAsync(user, roles, ct);

    var tk = tokens.GenerateTokens(user.Id, user.UserName!, user.FullName, roles, permissions);

    return new TokenResponse(tk.AccessToken, tk.ExpiresAt, tk.RefreshToken, tk.RefreshExpiresAt);
  }
}