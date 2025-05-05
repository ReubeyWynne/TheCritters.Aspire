using System.Collections.Immutable;

namespace TheCritters.Aspire.Domain.Aggregates;

public class Guild
{

    #region Serialisation Constructor
    protected Guild() { }
    #endregion

    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Name { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public List<Guid> AllowedLodges { get; private set; } = [];
    public DateTimeOffset CreatedAt { get; private set; }

    public static Guild Create(GuildCreated @event)
    {
        var guild = new Guild();
        guild.Apply(@event);
        return guild;
    }

    public void Apply(GuildCreated @event)
    {
        Name = @event.Name;
        Description = @event.Description;
        CreatedAt = @event.CreatedAt;
    }

    public void Apply(GuildJoinedLodge @event) => AllowedLodges.Add(@event.LodgeId);

    public void Apply(GuildLeftLodge @event) => AllowedLodges.Remove(@event.LodgeId);


    public static ImmutableArray<Type> EventTypes =>
        [
                typeof(GuildCreated),
                typeof(GuildJoinedLodge),
                typeof(GuildLeftLodge),
          ];
    public record GuildCreated(
        Guid Id,
        string Name,
        string Description,
        DateTimeOffset CreatedAt);

    public record GuildJoinedLodge(
        Guid GuildId,
        Guid LodgeId,
        DateTimeOffset GrantedAt);

    public record GuildLeftLodge(
        Guid GuildId,
        Guid LodgeId,
        DateTimeOffset RevokedAt);
}