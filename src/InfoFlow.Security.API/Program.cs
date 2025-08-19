using InfoFlow.Bootstrap.ApplicationBuilder;
using InfoFlow.Bootstrap.Observability.ServiceCollection;
using InfoFlow.Bootstrap.ServiceCollection;
using InfoFlow.Domain.Security.Entities;
using InfoFlow.Infrastructure;               // AddApplicationServices()
using InfoFlow.Persistence;                 // AddPersistenceServices()
using InfoFlow.Persistence.DbContexts;
using InfoFlow.Security.API.Startup.HostedServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using InfoFlow.Shared.Security.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);
var env = builder.Environment;
var config = builder.Configuration;

// DB Security
builder.Services.AddDbContext<SecurityDbContext>(opt =>
  opt.UseNpgsql(config.GetConnectionString("Postgres")!)
    .UseSnakeCaseNamingConvention());

// Identity (somente na Security API)
builder.Services.AddIdentityCore<AppUser>(opt =>
  {
    opt.Password.RequiredLength = 6;
    opt.Password.RequireDigit = true;
    opt.Password.RequireNonAlphanumeric = false;
    opt.Password.RequireUppercase = false;
    opt.Password.RequireLowercase = true;
    opt.User.RequireUniqueEmail = true;
  })
  .AddRoles<AppRole>()
  .AddEntityFrameworkStores<SecurityDbContext>()
  .AddDefaultTokenProviders();

// Bootstrap Core (JWT, Swagger, Versioning, CORS, MassTransit, Policies)
builder.Services.AddInfoFlowApiCore(
  config,
  env,
  serviceName: "InfoFlow.Security.API",
  permissionsForPolicies: Permissions.All // usa o enum/lista central de permissões
);

// Infra & Persistence
builder.Services.AddApplicationServices();   // TokenService, IdentityService, PermissionProvider...
builder.Services.AddPersistenceServices();   // EfRefreshTokenService, etc.

// Hosted seeds (admin bootstrap)
builder.Services.AddHostedService<SeedAdminHostedService>();

builder.Services.AddInfoFlowOpenTelemetry(builder.Configuration, "InfoFlow.Security.API");
builder.Logging.AddInfoFlowLogging();
var app = builder.Build();
app.UseInfoFlowPipeline(env);
app.Run();