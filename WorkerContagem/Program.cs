using Microsoft.Extensions.Configuration;
using WorkerContagem;
using WorkerContagem.Data;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using WorkerContagem.Tracing;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services.AddSingleton<ContagemRepository>();
        services.AddOpenTelemetry()
            .WithTracing(builder =>
            {
                builder
                    .AddSource(OpenTelemetryExtensions.ServiceName)
                    .SetResourceBuilder(
                        ResourceBuilder.CreateDefault()
                            .AddService(serviceName: OpenTelemetryExtensions.ServiceName,
                                serviceVersion: OpenTelemetryExtensions.ServiceVersion))
                    .AddAspNetCoreInstrumentation()
                    .AddSqlClientInstrumentation(
                        options => options.SetDbStatementForText = true)
                    .AddOtlpExporter(opts => opts.Endpoint = new Uri("http://localhost:4317"));
            });

        services.AddHostedService<Worker>();
    })
    .Build();

await host.RunAsync();