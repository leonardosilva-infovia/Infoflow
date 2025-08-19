using FluentValidation;
using InfoFlow.Application.Security.Abstractions;
using MediatR;
using InfoFlow.Domain.Security.Entities;
using Microsoft.AspNetCore.Identity;

namespace InfoFlow.Application.Security.Commands;

public sealed record RemoveUserFromRoleCommand(Guid UserId, string RoleName) : IRequest<Unit>;

public class RemoveUserFromRoleValidator : AbstractValidator<RemoveUserFromRoleCommand>
{
  public RemoveUserFromRoleValidator()
  {
    RuleFor(x => x.UserId).NotEmpty();
    RuleFor(x => x.RoleName).NotEmpty();
  }
}

public class RemoveUserFromRoleHandler(
  IIdentityService identity,
  UserManager<AppUser> userManager
) : IRequestHandler<RemoveUserFromRoleCommand, Unit>
{
  public async Task<Unit> Handle(RemoveUserFromRoleCommand request, CancellationToken ct)
  {
    var user = await identity.FindByIdAsync(request.UserId)
               ?? throw new KeyNotFoundException("Usuário não encontrado.");

    var result = await userManager.RemoveFromRoleAsync(user, request.RoleName);
    if (!result.Succeeded)
      throw new InvalidOperationException(string.Join("; ", result.Errors.Select(e => e.Description)));

    return Unit.Value;
  }
}