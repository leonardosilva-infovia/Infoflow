using FluentValidation;
using InfoFlow.Application.Security.Abstractions;
using InfoFlow.Shared.Security.DTOs;
using MediatR;
using Microsoft.AspNetCore.Identity;
using InfoFlow.Domain.Security.Entities;

namespace InfoFlow.Application.Security.Commands;

public sealed record CreateRoleCommand(CreateRoleRequest Input) : IRequest<RoleDto>;

public class CreateRoleValidator : AbstractValidator<CreateRoleCommand>
{
  public CreateRoleValidator()
  {
    RuleFor(x => x.Input.Name).NotEmpty().MaximumLength(128);
  }
}

public class CreateRoleHandler(
  IIdentityService identity,
  RoleManager<AppRole> roleManager
) : IRequestHandler<CreateRoleCommand, RoleDto>
{
  public async Task<RoleDto> Handle(CreateRoleCommand request, CancellationToken ct)
  {
    // Se já existe, apenas retorna
    var existing = await roleManager.FindByNameAsync(request.Input.Name);
    if (existing is not null)
      return new RoleDto(existing.Id, existing.Name!);

    var role = new AppRole { Name = request.Input.Name };
    var result = await roleManager.CreateAsync(role);

    if (!result.Succeeded)
      throw new InvalidOperationException(string.Join("; ", result.Errors.Select(e => e.Description)));

    // Sem re-buscar: o próprio objeto tem o Id preenchido
    return new RoleDto(role.Id, role.Name!);
  }
}