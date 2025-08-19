using FluentValidation;
using InfoFlow.Application.Security.Abstractions;
using InfoFlow.Shared.Security.DTOs;
using MediatR;

namespace InfoFlow.Application.Security.Commands;

public record ChangePasswordCommand(Guid UserId, ChangePasswordRequest Input) : IRequest<Unit>;

public class ChangePasswordValidator : AbstractValidator<ChangePasswordCommand>
{
  public ChangePasswordValidator()
  {
    RuleFor(x => x.UserId).NotEmpty();
    RuleFor(x => x.Input.CurrentPassword).NotEmpty();
    RuleFor(x => x.Input.NewPassword).NotEmpty().MinimumLength(8);
  }
}

// ⬇⬇⬇ ajuste aqui: IRequestHandler<ChangePasswordCommand, Unit>
public class ChangePasswordHandler(IIdentityService identity)
  : IRequestHandler<ChangePasswordCommand, Unit>
{
  // ⬇⬇⬇ ajuste aqui: Task<Unit> e return Unit.Value
  public async Task<Unit> Handle(ChangePasswordCommand request, CancellationToken ct)
  {
    var user = await identity.FindByIdAsync(request.UserId)
               ?? throw new KeyNotFoundException("Usuário não encontrado.");

    var (ok, errors) = await identity.ChangePasswordAsync(
      user, request.Input.CurrentPassword, request.Input.NewPassword);

    if (!ok) throw new InvalidOperationException(string.Join("; ", errors));

    return Unit.Value;
  }
}