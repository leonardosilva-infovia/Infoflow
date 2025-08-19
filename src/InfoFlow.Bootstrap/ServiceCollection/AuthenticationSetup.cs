using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace InfoFlow.Bootstrap.ServiceCollection;

public static class AuthenticationSetup
{
  public static IServiceCollection AddInfoFlowJwtAuthentication(this IServiceCollection services, IConfiguration config)
  {
    var issuer = config["Jwt:Issuer"];
    var audience = config["Jwt:Audience"];
    var key = config["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key não configurado.");

    var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));

    services.AddAuthentication(opt =>
      {
        opt.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
      })
      .AddJwtBearer(opt =>
      {
        opt.RequireHttpsMetadata = false;
        opt.SaveToken = true;
        opt.TokenValidationParameters = new TokenValidationParameters
        {
          ValidateIssuer = true,
          ValidateAudience = true,
          ValidateIssuerSigningKey = true,
          ValidIssuer = issuer,
          ValidAudience = audience,
          IssuerSigningKey = signingKey,
          ClockSkew = TimeSpan.FromSeconds(30)
        };
      });

    return services;
  }
}