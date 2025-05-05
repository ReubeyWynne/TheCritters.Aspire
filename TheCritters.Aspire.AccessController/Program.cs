using Marten;
using TheCritters.Aspire.AccessController.Services;
using TheCritters.Aspire.Infrastructure.Persistence;
using TheCritters.Aspire.ServiceDefaults;
using Wolverine;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults from Aspire
builder.AddServiceDefaults();

// Configure Marten
var postgresConnectionString = builder.Configuration.GetConnectionString("Postgres");
if (string.IsNullOrEmpty(postgresConnectionString))
{
    throw new InvalidOperationException("PostgreSQL connection string is missing from configuration");
}

builder.Services.AddMarten(options =>
{
    options.Connection(postgresConnectionString);
    options.DatabaseSchemaName = "critters";
    MartenConfiguration.ConfigureMarten(options, Weasel.Core.AutoCreate.CreateOnly);
})
.UseLightweightSessions();

// Configure Wolverine.Http
builder.Host.UseWolverine(opts =>
{
    opts.Discovery.IncludeAssembly(typeof(Program).Assembly);
});

// Add the WorkerManager as a singleton
builder.Services.AddSingleton<WorkerManager>();


var app = builder.Build();


app.UseHttpsRedirection();
app.MapDefaultEndpoints();

// Add simple minimal API endpoints as a backup/alternative
app.MapGet("/minimal/start/{lodgeId}", async (Guid lodgeId, WorkerManager workerManager) =>
{
    var result = await workerManager.StartWorkerForLodge(lodgeId);
    return result
        ? Results.Ok($"Started monitoring for Lodge {lodgeId}")
        : Results.Problem($"Failed to start monitoring for Lodge {lodgeId}");
});

app.MapGet("/minimal/stop/{lodgeId}", async (Guid lodgeId, WorkerManager workerManager) =>
{
    var result = await workerManager.StopWorkerForLodge(lodgeId);
    return result
        ? Results.Ok($"Stopped monitoring for Lodge {lodgeId}")
        : Results.NotFound($"No active monitoring for Lodge {lodgeId}");
});

app.Run();