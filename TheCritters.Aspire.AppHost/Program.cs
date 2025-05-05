var builder = DistributedApplication.CreateBuilder(args);

// Add PostgreSQL for Marten
var postgres = builder.AddPostgres("postgres")
   .WithDataVolume()
   .WithPgAdmin();

var db = postgres.AddDatabase("critters");

// Add Web UI Blazor with PostgreSQL dependency
var webui = builder.AddProject<Projects.TheCritters_Aspire_WebUI_Blazor>("critters-aspire-webui-blazor")
   .WithReference(db, "Postgres");

//// Add Access Controller with PostgreSQL dependency
var accessController = builder.AddProject<Projects.TheCritters_Aspire_AccessController>("critters-aspire-accesscontroller")
   .WithReference(db, "Postgres");

builder.Build().Run();