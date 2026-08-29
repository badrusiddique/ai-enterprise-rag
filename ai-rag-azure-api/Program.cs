using System.Diagnostics;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: true);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddMemoryCache();
builder.Services.AddControllers();
builder.Services.AddSemanticFilterConfiguration();
builder.Services.AddTelemetryConfiguration(builder.Configuration);
builder.Services.AddAzureOpenAIConfiguration(builder.Configuration);

builder.Services.AddHealthChecks();
builder.Services.AddHttpClient();
builder.Services.AddSingleton<IAzureOpenAiService, AzureOpenAiService>();
builder.Services.AddSingleton<IAzureAiSearchService, AzureAiSearchService>();
builder.Services.AddSingleton<IContentSafetyService, AzureContentSafetyService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.WithOpenApiRoutePattern("/openapi/v1.json");
    });
}

app.MapHealthChecks("/health");

app.Use(async (context, next) =>
{
    var sessionId = context.Request.Headers["X-Session-ID"].FirstOrDefault() ?? Guid.NewGuid().ToString();

    Activity.Current?.SetTag("session.id", sessionId);
    Activity.Current?.SetTag("application.id", "ai-rag-azure-api");
    Activity.Current?.SetTag("user.id", context.User?.Identity?.Name ?? "anonymous");

    context.Items["SessionId"] = sessionId;

    await next.Invoke();
});

app.MapControllers();

app.UseHttpsRedirection();

app.Run();
