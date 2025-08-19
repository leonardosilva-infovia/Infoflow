using InfoFlow.Bootstrap.ApplicationBuilder;
using InfoFlow.Bootstrap.ServiceCollection;
using InfoFlow.Persistence;
using InfoFlow.Persistence.DbContexts; // se houver DbContext aqui também
using InfoFlow.Shared.Security.Authorization;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var env = builder.Environment;
var config = builder.Configuration;

// DbContext dessa API (schema próprio)
builder.Services.AddDbContext<TasksDbContext>(opt =>
  opt.UseNpgsql(config.GetConnectionString("Postgres")!)
    .UseSnakeCaseNamingConvention());

// Bootstrap Core padrão (JWT, Swagger, CORS, Versioning, MassTransit, Policies)
builder.Services.AddInfoFlowApiCore(
  config,
  env,
  serviceName: "InfoFlow.Tasks.API",
  permissionsForPolicies: Permissions.All // ou um subconjunto específico desta API
);

// Registrar serviços da API (Application/Infrastructure)...
// builder.Services.AddApplicationServicesForTasks(); // (separar por camadas conforme sua org)

var app = builder.Build();
app.UseInfoFlowPipeline(env);
app.Run();