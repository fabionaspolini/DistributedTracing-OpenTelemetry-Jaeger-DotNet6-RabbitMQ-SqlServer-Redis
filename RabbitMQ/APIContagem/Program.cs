using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using APIContagem.Tracing;
using APIContagem.Messaging;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddOpenTelemetry()
    .WithTracing(builder =>
    {
        builder
            .AddSource(OpenTelemetryExtensions.ServiceName)
            .SetResourceBuilder(
                ResourceBuilder.CreateDefault()
                    .AddService(serviceName: OpenTelemetryExtensions.ServiceName,
                        serviceVersion: OpenTelemetryExtensions.ServiceVersion))
            .AddHttpClientInstrumentation()
            .AddAspNetCoreInstrumentation()
            .AddOtlpExporter(opts => opts.Endpoint = new Uri("http://localhost:4317"));
    });

builder.Services.AddScoped<MessageSender>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();