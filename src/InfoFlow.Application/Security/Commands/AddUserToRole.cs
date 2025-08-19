using FluentValidation;
using InfoFlow.Application.Security.Abstractions;
using MediatR;

namespace InfoFlow.Application.Security.Commands;

public sealed record AddUserToRoleCommand(Guid UserId, string RoleName) : IRequest<Unit>;

public class AddUserToRoleValidator : AbstractValidator<AddUserToRoleCommand>
{
  public AddUserToRoleValidator()
  {
    RuleFor(x => x.UserId).NotEmpty();
    RuleFor(x => x.RoleName).NotEmpty();
  }
}

public class AddUserToRoleHandler(IIdentityService identity)
  : IRequestHandler<AddUserToRoleCommand, Unit>
{
  public async Task<Unit> Handle(AddUserToRoleCommand request, CancellationToken ct)
  {
    var user = await identity.FindByIdAsync(request.UserId)
               ?? throw new KeyNotFoundException("Usuário não encontrado.");

    var (ok, errors) = await identity.AddUserToRoleAsync(user, request.RoleName);
    if (!ok) throw new InvalidOperationException(string.Join("; ", errors));

    return Unit.Value;
  }
}