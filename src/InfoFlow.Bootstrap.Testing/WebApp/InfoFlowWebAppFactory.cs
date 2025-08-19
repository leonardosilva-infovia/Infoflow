using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace InfoFlow.Bootstrap.Testing.WebApp;

public class InfoFlowWebAppFactory<TEntryPoint> : WebApplicationFactory<TEntryPoint>
  where TEntryPoint : class
{
  private readonly Dictionary<string, string?> _overrides = new();

  public InfoFlowWebAppFactory<TEntryPoint> WithSetting(string key, string? value)
  {
    _overrides[key] = value;
    return this;
  }

  protected override IHost CreateHost(IHostBuilder builder)
  {
    builder.ConfigureAppConfiguration(cfg => cfg.AddInMemoryCollection(_overrides));
    return base.CreateHost(builder);
  }

  public HttpClient CreateClientJson(string? bearer = null)
  {
    var client = CreateClient(new WebApplicationFactoryClientOptions
    {
      AllowAutoRedirect = false
    });
    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    if (!string.IsNullOrWhiteSpace(bearer))
      client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearer);
    return client;
  }
}