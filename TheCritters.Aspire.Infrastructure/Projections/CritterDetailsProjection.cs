using Marten.Events.Aggregation;
using static TheCritters.Aspire.Domain.Aggregates.Critter.Events;

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
    public static CritterDetails Create(Registered @event)
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

    public static void Apply(JoinedGuild @event, CritterDetails view)
    {
        view.GuildIds.Add(@event.GuildId);
    }

    public static void Apply(LeftGuild @event, CritterDetails view)
    {
        view.GuildIds.Remove(@event.GuildId);
    }

   
}