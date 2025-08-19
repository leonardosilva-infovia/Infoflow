using Microsoft.EntityFrameworkCore;

namespace InfoFlow.Persistence.DbContexts;

public class TasksDbContext(DbContextOptions<TasksDbContext> options) : DbContext(options)
{
  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.HasDefaultSchema("tasks");
    base.OnModelCreating(modelBuilder);
  }
}