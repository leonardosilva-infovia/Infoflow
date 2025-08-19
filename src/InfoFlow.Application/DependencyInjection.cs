using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace InfoFlow.Application;

public static class DependencyInjection
{
  public static IServiceCollection AddApplication(this IServiceCollection services)
  {
    var asm = Assembly.GetExecutingAssembly();

    // MediatR 11: registra handlers via overload antigo
    services.AddMediatR(asm);

    // FluentValidation continua igual
    services.AddValidatorsFromAssembly(asm);

    return services;
  }
}