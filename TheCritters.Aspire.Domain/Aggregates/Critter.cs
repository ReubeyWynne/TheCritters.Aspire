using System.Collections.Immutable;

namespace TheCritters.Aspire.Domain.Aggregates;

public class Critter
{
    #region Serialisation Constructor
    protected Critter() { }
    #endregion

    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Name { get; private set; } = null!;
    public string Species { get; private set; } = null!;
    public List<Guid> Guilds { get; private set; } = [];
    public Guid? FamilyId { get; private set; }
    public DateTimeOffset RegisteredAt { get; private set; }
    public bool IsActive { get; private set; }

    public static Critter Create(CritterRegistered @event)
    {
        Critter critter = new();
        critter.Apply(@event);
        return critter;
    }

    public void Apply(CritterRegistered @event)
    {
        Name = @event.Name;
        Species = @event.Species;
        RegisteredAt = @event.RegisteredAt;
        IsActive = true;
    }

    public void Apply(CritterJoinedGuild @event) => Guilds.Add(@event.GuildId);

    public void Apply(CritterLeftGuild @event) => Guilds.Remove(@event.GuildId);

    public static ImmutableArray<Type> EventTypes =>
       [
           typeof(CritterRegistered),
            typeof(CritterJoinedGuild),
            typeof(CritterLeftGuild)
       ];
    public record CritterRegistered(
    Guid Id,
    string Name,
    string Species,
    DateTimeOffset RegisteredAt);

    public record CritterJoinedGuild(
    Guid CritterId,
    Guid GuildId,
    DateTimeOffset JoinedAt);

    public record CritterLeftGuild(
    Guid CritterId,
    Guid GuildId,
    DateTimeOffset LeftAt);


}