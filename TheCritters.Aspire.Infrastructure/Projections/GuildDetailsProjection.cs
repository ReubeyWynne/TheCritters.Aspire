using Marten.Events.Aggregation;
using static TheCritters.Aspire.Domain.Aggregates.Guild.Events;

namespace TheCritters.Aspire.Infrastructure.Projections;

public class GuildDetails
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<Guid> AllowedLodges { get; set; } = [];
    public DateTimeOffset CreatedAt { get; set; }
}

public class GuildProjection : SingleStreamProjection<GuildDetails>
{
    public static GuildDetails Create(Created @event)
    {
        return new GuildDetails
        {
            Name = @event.Name,
            Description = @event.Description,
            CreatedAt = @event.CreatedAt,
            AllowedLodges = []
        };
    }

    public static void Apply(AccessGranted @event, GuildDetails view)
    {
        view.AllowedLodges.Add(@event.LodgeId);
    }

    public static void Apply(AccessRevoked @event, GuildDetails view)
    {
        view.AllowedLodges.Remove(@event.LodgeId);
    }
}