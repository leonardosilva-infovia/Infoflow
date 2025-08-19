using FluentValidation;
using InfoFlow.Application.Security.Abstractions;
using InfoFlow.Domain.Security.Entities;
using InfoFlow.Shared.Security.DTOs;
using MediatR;

namespace InfoFlow.Application.Security.Commands;

public record RegisterUserCommand(RegisterRequest Input) : IRequest<UserDto>;

public class RegisterUserValidator : AbstractValidator<RegisterUserCommand>
{
  public RegisterUserValidator()
  {
    RuleFor(x => x.Input.FullName).NotEmpty().MaximumLength(200);
    RuleFor(x => x.Input.Email).NotEmpty().EmailAddress();
    RuleFor(x => x.Input.Password).NotEmpty().MinimumLength(8);
  }
}

public class RegisterUserHandler(IIdentityService identity)
  : IRequestHandler<RegisterUserCommand, UserDto>
{
  public async Task<UserDto> Handle(RegisterUserCommand request, CancellationToken ct)
  {
    var existing = await identity.FindByEmailAsync(request.Input.Email);
    if (existing is not null) throw new InvalidOperationException("E-mail já registrado.");

    var user = new AppUser
    {
      UserName = request.Input.Email,
      Email = request.Input.Email,
      FullName = request.Input.FullName,
      EmailConfirmed = false, // pode confirmar depois por e-mail
      IsActive = true
    };

    var (ok, errors) = await identity.CreateUserAsync(user, request.Input.Password);
    if (!ok) throw new InvalidOperationException(string.Join("; ", errors));

    var roles = await identity.GetUserRolesAsync(user);
    return new UserDto(user.Id, user.Email!, user.FullName, user.IsActive, roles);
  }
}