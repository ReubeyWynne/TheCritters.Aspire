using Marten.Events.Aggregation;
using static TheCritters.Aspire.Domain.Aggregates.Critter;

namespace TheCritters.Aspire.Infrastructure.Projections;

public class CritterDetails
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Species { get; set; } = string.Empty;
    public List<Guid> GuildIds { get; set; } = [];
    public Guid? FamilyId { get; set; }
    public DateTimeOffset RegisteredAt { get; set; }
    public bool IsActive { get; set; }
}

public class CritterDetailsProjection : SingleStreamProjection<CritterDetails>
{
    public static CritterDetails Create(CritterRegistered @event)
    {
        return new CritterDetails
        {
            Name = @event.Name,
            Species = @event.Species,
            RegisteredAt = @event.RegisteredAt,
            IsActive = true,
            GuildIds = []
        };
    }

    public static void Apply(CritterJoinedGuild @event, CritterDetails view)
    {
        view.GuildIds.Add(@event.GuildId);
    }

    public static void Apply(CritterLeftGuild @event, CritterDetails view)
    {
        view.GuildIds.Remove(@event.GuildId);
    }

   
}