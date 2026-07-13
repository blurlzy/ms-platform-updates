using Azure.Extensions.AspNetCore.Configuration.Secrets;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using MS_Updates.Auth;
using MS_Updates.Extensions;
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

// Add services to the container.
// load cosmos configs
var cosmosConn = builder.Configuration[SecretKeys.CosmosConnection];
var cosmosDb = builder.Configuration[SecretKeys.CosmosDb];
var cosmosContainer = builder.Configuration[SecretKeys.CosmosContainer];
// cosmos db context
#pragma warning disable CS8604 // Possible null reference argument.
builder.Services.AddSingleton(new CosmosDataService(cosmosConn, cosmosDb, cosmosContainer));
#pragma warning restore CS8604 // Possible null reference argument.

// cors
string[] allowedOrigins = new[]
                    {
                         "http://localhost:4200"
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
app.MapGet("/api/ms-updates", async (CosmosDataService cosmosDataService, string? source = null, int pageIndex = 0, int pageSize = 12) =>
{
     var updates = string.IsNullOrWhiteSpace(source)
          ? await cosmosDataService.ListUpdatesAsync(pageIndex, pageSize)
          : await cosmosDataService.ListUpdatesAsync(source, pageIndex, pageSize);

     return Results.Ok(updates);
});


app.Run();


