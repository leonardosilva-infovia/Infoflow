using FluentValidation;
using InfoFlow.Shared.Security.DTOs;
using MediatR;

namespace InfoFlow.Application.Security.Commands;

/// <summary>Handler fica na API (infra) por depender do DbContext para RefreshToken.
/// Aqui só definimos o contrato.</summary>
public record RefreshTokenCommand(RefreshRequest Input) : IRequest<TokenResponse>;

public class RefreshTokenValidator : AbstractValidator<RefreshTokenCommand>
{
  public RefreshTokenValidator()
  {
    RuleFor(x => x.Input.RefreshToken).NotEmpty().MaximumLength(200);
  }
}