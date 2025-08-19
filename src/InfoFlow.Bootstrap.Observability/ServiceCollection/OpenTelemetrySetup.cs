using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace InfoFlow.Bootstrap.Observability.ServiceCollection;

public static class OpenTelemetrySetup
{
    public static IServiceCollection AddInfoFlowOpenTelemetry(
        this IServiceCollection services,
        IConfiguration config,
        string serviceName,
        string? serviceVersion = null)
    {
        var resource = ResourceBuilder.CreateDefault()
            .AddService(serviceName: serviceName, serviceVersion: serviceVersion ?? "1.0.0")
            .AddAttributes(new[]
            {
                new KeyValuePair<string, object>(
                    "deployment.environment",
                    config["ASPNETCORE_ENVIRONMENT"] ?? "Development")
            });

        // Endereços do Collector (OTLP) via appsettings
        var otlpEndpoint = config["OpenTelemetry:Otlp:Endpoint"] ?? "http://localhost:4317"; // gRPC padrão do Collector

        services.AddOpenTelemetry()
            .ConfigureResource(rb => rb.AddService(serviceName))
            .WithTracing(b =>
            {
                b.SetResourceBuilder(resource);
                b.AddAspNetCoreInstrumentation(o => o.RecordException = true);
                b.AddHttpClientInstrumentation();

                // Exporta para o Collector (OTLP gRPC)
                b.AddOtlpExporter(o =>
                {
                    o.Endpoint = new Uri(otlpEndpoint);
                });
            })
            .WithMetrics(b =>
            {
                b.SetResourceBuilder(resource);
                b.AddAspNetCoreInstrumentation();
                b.AddHttpClientInstrumentation();
                b.AddRuntimeInstrumentation();

                // Exporta métricas para o Collector (OTLP gRPC)
                b.AddOtlpExporter(o =>
                {
                    o.Endpoint = new Uri(otlpEndpoint);
                });
            });

        return services;
    }
}