using Microsoft.EntityFrameworkCore;

namespace InfoFlow.Persistence.DbContexts;

public class ProcessesDbContext(DbContextOptions<ProcessesDbContext> options) : DbContext(options)
{
  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.HasDefaultSchema("processes");
    base.OnModelCreating(modelBuilder);
  }
}