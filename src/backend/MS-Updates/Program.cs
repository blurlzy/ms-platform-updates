using Azure.Extensions.AspNetCore.Configuration.Secrets;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using MS_Updates.Auth;
using MS_Updates.Extensions;
using MS_Updates.Filters;
using MS_Updates.Persistence;

var builder = WebApplication.CreateBuilder(args);

// register secret client
SecretClient secretClient = new SecretClient(new Uri($"https://{builder.Configuration["Azure:KeyVault"]}.vault.azure.net"),
                                              new DefaultAzureCredential(new DefaultAzureCredentialOptions
                                              {
                                                   ExcludeEnvironmentCredential = true,
                                                   ExcludeVisualStudioCodeCredential = true,
                                                   ExcludeInteractiveBrowserCredential = true,
                                              }));
// loads secrets into configuration. ## it requres Azure.Extensions.AspNetCore.Configuration.Secrets package
builder.Configuration.AddAzureKeyVault(secretClient, new KeyVaultSecretManager());

// load configs from key vault
var cosmosConn = builder.Configuration[SecretKeys.CosmosConnection];
var cosmosDb = builder.Configuration[SecretKeys.CosmosDb];
var cosmosContainer = builder.Configuration[SecretKeys.CosmosContainer];
var appInsightsConnection = builder.Configuration[SecretKeys.AppInsightsConnection];

if (cosmosConn == null || cosmosDb == null || cosmosContainer == null)
{
     throw new InvalidOperationException("Cosmos DB configuration is missing.");
}

if (appInsightsConnection == null)
{
     throw new InvalidOperationException("Application Insights configuration is missing.");
}

// Add services to the container.
// cosmos data service
builder.Services.AddSingleton(new CosmosDataService(cosmosConn, cosmosDb, cosmosContainer));

// cache service
builder.Services.AddSingleton<CosmosCacheService>();

// cache - install # Microsoft.Extensions.Caching.Hybrid
builder.Services.AddHybridCache();

// app insights - install # Microsoft.ApplicationInsights.AspNetCore
var options = new ApplicationInsightsServiceOptions { ConnectionString = appInsightsConnection };
builder.Services.AddApplicationInsightsTelemetry(options: options);

// cors
string[] allowedOrigins = new[]
                    {
                         "http://localhost:4200",
                         "https://lively-moss-0c406fb00.7.azurestaticapps.net",
                         "https://msupdates.zongyi.me"
                    };

// cors policy
builder.Services.AddCors(
        opt =>
        {
             opt.AddPolicy("allowCors",
             builder => builder.WithOrigins(allowedOrigins).AllowAnyHeader().AllowAnyMethod());
        });


var app = builder.Build();

// Configure the HTTP request pipeline.
// error handling pipeline (middleware)
app.UseGlobalExceptionHandler(app.Logger);
app.UseHttpsRedirection();
app.UseCors("allowCors");

// map endpoints
// GET /api/ms-updates?source=xxx&pageIndex=0&pageSize=12
app.MapGet("/api/ms-updates", async (CosmosCacheService cosmosCache, string? source = null, int pageIndex = 0, int pageSize = 12) =>
{
     var updates = string.IsNullOrWhiteSpace(source)
          ? await cosmosCache.ListUpdatesAsync(pageIndex, pageSize)
          : await cosmosCache.ListUpdatesAsync(source, pageIndex, pageSize);

     return Results.Ok(updates);
})
.AddEndpointFilter<PagingValidationFilter>()
.AddEndpointFilter<SourceValidationFilter>();

// GET / (root endpoint for health check)
app.MapGet("/", () =>
{
     return Results.Ok(new { name= "MS Cloud & AI Updates API", version = "v1.0.0.20260718" });
});

app.Run();


