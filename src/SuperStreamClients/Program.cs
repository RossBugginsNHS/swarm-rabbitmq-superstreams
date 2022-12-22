global using RabbitMQ.Stream.Client;
global using System.Net;
global using Microsoft.Extensions.Options;
using SuperStreamClients;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSwarmSuperStream(
    builder.Configuration.GetSection(RabbitMqStreamOptions.Name),
    options =>
    {
    });

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
var options = app.Services.GetService<IOptions<RabbitMqStreamOptions>>();

if (options.Value.Metrics)
    app.UseOpenTelemetryPrometheusScrapingEndpoint();

if (options.Value.Analytics && app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

if(options.Value.Analytics)
    app.MapControllers();

app.Run();
