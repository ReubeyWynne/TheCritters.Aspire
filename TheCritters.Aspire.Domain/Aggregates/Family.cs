using System.Collections.Immutable;
using static TheCritters.Aspire.Domain.Aggregates.Family.Events;

namespace TheCritters.Aspire.Domain.Aggregates;

public class Family
{
    #region Serialisation Constructor
    protected Family() { }
    #endregion

    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Name { get; private set; } = null!;
    public string Motto { get; private set; } = null!;
    public DateTimeOffset EstablishedAt { get; private set; }
    public List<Guid> AssociatedLodges { get; private set; } = [];
    public List<Guid> FamilyIds { get; private set; } = [];
    public int FamilyCount { get; private set; } 
    public static Family Create(string name, string motto, DateTimeOffset establishedAt) => new()
    {
        Name = name,
        Motto = motto,
        EstablishedAt = establishedAt
    };
    public void Apply(Established @event)
    {
        Name = @event.Name;
        Motto = @event.Motto;
        EstablishedAt = @event.EstablishedAt;
    }

    public void Apply(CritterJoined @event)
    {
        FamilyCount++;
        FamilyIds.Add(@event.CritterId);
    }

    public void Apply(JoinedLodge @event)
    {
        AssociatedLodges.Add(@event.LodgeId);
    }

    public void Apply(LeftLodge @event)
    {
        AssociatedLodges.Remove(@event.LodgeId);
    }

    public static class Events
    {
        public static ImmutableArray<Type> Types =>
        [
            typeof(Established),
            typeof(JoinedLodge),
            typeof(LeftLodge)
        ];
        public record Established(
            Guid Id,
            string Name,
            string Motto,
            DateTimeOffset EstablishedAt);

        public record JoinedLodge(
            Guid LodgeId);

        public record LeftLodge(
            Guid LodgeId);

        public record CritterJoined(
            Guid FamilyId,
            Guid CritterId,
            DateTimeOffset JoinedAt);
    }
}