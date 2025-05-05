using TheCritters.Aspire.Infrastructure.Persistence;
using TheCritters.Aspire.ServiceDefaults;
using JasperFx.Core;
using Marten;
using Npgsql;
using Scalar.AspNetCore;
using Wolverine;
using Wolverine.ErrorHandling;
using Wolverine.Http;
using Wolverine.Marten;
using MudBlazor.Services;
using Oakton;
using TheCritters.Aspire.WebUI.Blazor;
using TheCritters.Aspire.Application.Critters.Commands;
using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Configure logging to reduce Npgsql noise
builder.Logging.AddFilter("Npgsql.Command", LogLevel.Warning);
builder.Logging.AddFilter("Npgsql", LogLevel.Warning);

// Validate connection string
var postgresConnectionString = builder.Configuration.GetConnectionString("Postgres");
if (string.IsNullOrEmpty(postgresConnectionString))
{
    throw new InvalidOperationException("PostgreSQL connection string is missing from configuration");
}

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddMudServices();


// Configure Marten
builder.Services.AddMarten(options =>
{
    options.Connection(postgresConnectionString);
    options.DatabaseSchemaName = "critters";
    MartenConfiguration.ConfigureMarten(options, Weasel.Core.AutoCreate.All);
})
.UseLightweightSessions()
.ApplyAllDatabaseChangesOnStartup()
.IntegrateWithWolverine();

// Configure Wolverine
builder.Host.UseWolverine(opts =>
{
    opts.Policies.AutoApplyTransactions();

    opts.Policies.UseDurableLocalQueues();
    opts.Discovery.IncludeAssembly(typeof(Program).Assembly);
    opts.Discovery.IncludeAssembly(typeof(JoinGuildCommand).Assembly);
    opts.OnException<PostgresException>().RetryWithCooldown(50.Milliseconds(), 100.Milliseconds(), 250.Milliseconds());
    var op = opts.DescribeHandlerMatch(typeof(JoinGuildCommand));
    Debug.WriteLine(op);
});

builder.Services.AddWolverineHttp();

// Add OpenAPI support
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, context, cancellationToken) =>
    {
        document.Info.Title = "Critters API";
        document.Info.Version = "v1";
        document.Info.Description = "API for managing critters, guilds, lodges, and access control";
        return Task.CompletedTask;
    });
});

var app = builder.Build();

app.MapDefaultEndpoints();
app.MapHealthChecks("/health");
app.MapWolverineEndpoints();

// Configure OpenAPI
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.Title = "Critters API";
        options.Theme = ScalarTheme.Default;
        options.WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
    });
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

await app.RunOaktonCommands(args);