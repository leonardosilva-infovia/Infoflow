using Microsoft.EntityFrameworkCore;

namespace InfoFlow.Persistence.DbContexts;

public class AutomationDbContext(DbContextOptions<AutomationDbContext> options) : DbContext(options)
{
  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.HasDefaultSchema("automation");
    base.OnModelCreating(modelBuilder);
  }
}