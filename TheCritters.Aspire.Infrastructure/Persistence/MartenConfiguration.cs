// src/TheCritters.Aspire.Infrastructure/Persistence/MartenConfiguration.cs
using TheCritters.Aspire.Infrastructure.Projections;
using Marten;
using Marten.Events.Projections;
using TheCritters.Aspire.Domain.Access;
using TheCritters.Aspire.Domain.Aggregates;
using Weasel.Core;

namespace TheCritters.Aspire.Infrastructure.Persistence;

public static class MartenConfiguration
{
    public static void ConfigureMarten(StoreOptions options, AutoCreate Create)
    {
        // Configure event sourcing
        options.Events.AddEventTypes(LodgeAccess.EventTypes);
        options.Events.AddEventTypes(Critter.EventTypes);
        options.Events.AddEventTypes(Guild.EventTypes);
        options.Events.AddEventTypes(Lodge.EventTypes);
        options.Events.AddEventTypes(Family.EventTypes);

        // Configure projections
        options.Projections.Add<CritterDetailsProjection>(ProjectionLifecycle.Inline);
        options.Projections.Add<LodgeOccupancyProjection>(ProjectionLifecycle.Inline);
        options.Projections.Add<GuildProjection>(ProjectionLifecycle.Inline);
        options.Projections.Add<FamilyDetailsProjection>(ProjectionLifecycle.Inline);
        options.Projections.Add<AccessDetailsProjection>(ProjectionLifecycle.Inline);

        options.Events.StreamIdentity = Marten.Events.StreamIdentity.AsGuid;
        options.Events.UseMandatoryStreamTypeDeclaration = true;


        options.AutoCreateSchemaObjects = Create;

    }
}