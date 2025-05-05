using System.Collections.Immutable;
using static TheCritters.Aspire.Domain.Aggregates.Critter.Events;

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

    public static Critter Create(Registered @event)
    {
        Critter critter = new();
        critter.Apply(@event);
        return critter;
    }

    public void Apply(Registered @event)
    {
        Name = @event.Name;
        Species = @event.Species;
        RegisteredAt = @event.RegisteredAt;
        IsActive = true;
    }

    public void Apply(JoinedGuild @event) => Guilds.Add(@event.GuildId);

    public void Apply(LeftGuild @event) => Guilds.Remove(@event.GuildId);

    public static class Events
    {
        public static ImmutableArray<Type> Types =>
        [
            typeof(Registered),
            typeof(JoinedGuild),
            typeof(LeftGuild)
        ];
        public record Registered(
        Guid Id,
        string Name,
        string Species,
        DateTimeOffset RegisteredAt);

        public record JoinedGuild(
        Guid CritterId,
        Guid GuildId,
        DateTimeOffset JoinedAt);

        public record LeftGuild(
        Guid CritterId,
        Guid GuildId,
        DateTimeOffset LeftAt);

    }





}