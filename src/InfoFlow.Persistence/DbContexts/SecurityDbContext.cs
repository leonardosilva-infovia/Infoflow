using InfoFlow.Domain.Security.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace InfoFlow.Persistence.DbContexts;

public class SecurityDbContext(DbContextOptions<SecurityDbContext> options)
  : IdentityDbContext<AppUser, AppRole, Guid,
    IdentityUserClaim<Guid>, IdentityUserRole<Guid>, IdentityUserLogin<Guid>,
    IdentityRoleClaim<Guid>, IdentityUserToken<Guid>>(options)
{
  public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.HasDefaultSchema("security");

    // Tabelas Identity em snake_case
    modelBuilder.Entity<AppUser>().ToTable("users");
    modelBuilder.Entity<AppRole>().ToTable("roles");
    modelBuilder.Entity<IdentityUserRole<Guid>>().ToTable("user_roles");
    modelBuilder.Entity<IdentityUserClaim<Guid>>().ToTable("user_claims");
    modelBuilder.Entity<IdentityUserLogin<Guid>>().ToTable("user_logins");
    modelBuilder.Entity<IdentityRoleClaim<Guid>>().ToTable("role_claims");
    modelBuilder.Entity<IdentityUserToken<Guid>>().ToTable("user_tokens");

    // RefreshToken
    modelBuilder.Entity<RefreshToken>(b =>
    {
      b.ToTable("refresh_tokens");
      b.HasKey(x => x.Id);
      b.Property(x => x.Token).HasMaxLength(200).IsRequired();
      b.Property(x => x.CreatedByIp).HasMaxLength(64);
      b.HasIndex(x => new { x.UserId, x.Token }).IsUnique();
    });

    base.OnModelCreating(modelBuilder);
  }
}