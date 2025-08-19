using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using InfoFlow.Domain.Security.Entities;
using InfoFlow.Persistence.DbContexts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace InfoFlow.Security.API.Startup.HostedServices;

public class SeedAdminHostedService(
    IServiceProvider sp,
    ILogger<SeedAdminHostedService> logger,
    IConfiguration config
) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = sp.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<SecurityDbContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<AppRole>>();

        // Garante que o banco está acessível
        await db.Database.MigrateAsync(cancellationToken);

        var email = config["AdminBootstrap:Email"];
        if (string.IsNullOrWhiteSpace(email))
        {
            logger.LogInformation("AdminBootstrap:Email não configurado. Seed de admin ignorado.");
            return;
        }

        // 1) Garante role 'admin'
        const string roleName = "admin";
        if (!await roleManager.RoleExistsAsync(roleName))
        {
            var createRole = await roleManager.CreateAsync(new AppRole { Name = roleName });
            if (!createRole.Succeeded)
            {
                var err = string.Join("; ", createRole.Errors.Select(e => e.Description));
                logger.LogError("Falha ao criar role 'admin': {Errors}", err);
                return;
            }
            logger.LogInformation("Role 'admin' criada.");
        }

        // 2) Procura usuário pelo e-mail e adiciona à role
        var user = await userManager.Users.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
        if (user is null)
        {
            logger.LogWarning("Usuário {Email} não encontrado para seed de admin.", email);
            return;
        }

        if (!await userManager.IsInRoleAsync(user, roleName))
        {
            var add = await userManager.AddToRoleAsync(user, roleName);
            if (!add.Succeeded)
            {
                var err = string.Join("; ", add.Errors.Select(e => e.Description));
                logger.LogError("Falha ao adicionar usuário {Email} à role 'admin': {Errors}", email, err);
                return;
            }
            logger.LogInformation("Usuário {Email} adicionado à role 'admin'.", email);
        }
        else
        {
            logger.LogInformation("Usuário {Email} já está na role 'admin'.", email);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}