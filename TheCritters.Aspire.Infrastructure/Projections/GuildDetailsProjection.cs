using Marten.Events.Aggregation;
using static TheCritters.Aspire.Domain.Aggregates.Guild;

namespace TheCritters.Aspire.Infrastructure.Projections;

public class GuildDetails
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<Guid> SubscribedLodges { get; set; } = [];
    public DateTimeOffset CreatedAt { get; set; }
}

public class GuildProjection : SingleStreamProjection<GuildDetails>
{
    public static GuildDetails Create(GuildCreated @event)
    {
        return new GuildDetails
        {
            Name = @event.Name,
            Description = @event.Description,
            CreatedAt = @event.CreatedAt,
            SubscribedLodges = []
        };
    }

    public static void Apply(GuildJoinedLodge @event, GuildDetails view)
    {
        view.SubscribedLodges.Add(@event.LodgeId);
    }

    public static void Apply(GuildLeftLodge @event, GuildDetails view)
    {
        view.SubscribedLodges.Remove(@event.LodgeId);
    }
}