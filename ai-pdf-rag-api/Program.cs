using System.Diagnostics;
using AiPdfRagApp.Configuration;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: true);
var localSettingsPath = Path.Combine(builder.Environment.ContentRootPath, "appsettings.Local.json");
var localSettingsLoaded = File.Exists(localSettingsPath);
var settingsProfile = Environment.GetEnvironmentVariable("APP_SETTINGS_PROFILE")
    ?? (localSettingsLoaded ? "Local" : builder.Environment.EnvironmentName);

var ragOptions = new RagOptions();
builder.Configuration.GetSection("Rag").Bind(ragOptions);
var langfuseOptions = new LangfuseOptions();
builder.Configuration.GetSection("Langfuse").Bind(langfuseOptions);

// Add services to the container.
builder.Services.AddKernel();
builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddHealthChecks();
builder.Services.AddHttpClient();

// Add RAG services from shared library
builder.Services.AddRagServices(ragOptions, langfuseOptions);

var app = builder.Build();

app.Logger.LogInformation("Environment: {EnvironmentName}", app.Environment.EnvironmentName);
app.Logger.LogInformation("Settings profile: {SettingsProfile}", settingsProfile);
app.Logger.LogInformation("Local settings: {LocalSettingsStatus}", localSettingsLoaded ? "loaded" : "not found");

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
    Activity.Current?.SetTag("user.id", context.User?.Identity?.Name ?? "anonymous");

    context.Items["SessionId"] = sessionId;

    await next.Invoke();
});

app.MapControllers();

app.UseHttpsRedirection();

app.Run();
