using JobSearchAssistant.Api.Auth;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = FunctionsApplication.CreateBuilder(args);

// Functions worker + HTTP
builder.ConfigureFunctionsWebApplication();

// DI
builder.Services.AddSingleton<JwtValidator>();

// Optional: Application Insights (tu peux le garder)
builder.Services
    .AddApplicationInsightsTelemetryWorkerService()
    .ConfigureFunctionsApplicationInsights();

var app = builder.Build();
app.Run();
