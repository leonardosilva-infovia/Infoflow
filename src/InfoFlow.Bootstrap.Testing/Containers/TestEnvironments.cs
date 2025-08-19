using System.Threading.Tasks;
using Testcontainers.PostgreSql;
using Testcontainers.RabbitMq;
using Xunit;

namespace InfoFlow.Bootstrap.Testing.Containers;

public sealed class TestEnvironments : IAsyncLifetime
{
  private PostgreSqlContainer _postgres = default!;
  private RabbitMqContainer _rabbit = default!;

  public string PostgresConnectionString => _postgres.GetConnectionString();
  public string RabbitMqConnectionString => _rabbit.GetConnectionString();

  public async Task InitializeAsync()
  {
    _postgres = new PostgreSqlBuilder()
      .WithImage("postgres:16-alpine")
      .WithDatabase("infoflow_test")
      .WithUsername("postgres")
      .WithPassword("postgres")
      .Build();

    _rabbit = new RabbitMqBuilder()
      .WithImage("rabbitmq:3-management-alpine")
      .Build();

    await _postgres.StartAsync();
    await _rabbit.StartAsync();
  }

  public async Task DisposeAsync()
  {
    await _rabbit.DisposeAsync();
    await _postgres.DisposeAsync();
  }
}