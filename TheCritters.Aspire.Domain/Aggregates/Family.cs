using System.Collections.Immutable;

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
    public void Apply(FamilyEstablished @event)
    {
        Name = @event.Name;
        Motto = @event.Motto;
        EstablishedAt = @event.EstablishedAt;
    }

    public void Apply(FamilyCritterJoined @event)
    {
        FamilyCount++;
        FamilyIds.Add(@event.CritterId);
    }

    public void Apply(FamilyJoinLodge @event)
    {
        AssociatedLodges.Add(@event.LodgeId);
    }

    public void Apply(FamilyLeftLodge @event)
    {
        AssociatedLodges.Remove(@event.LodgeId);
    }

    public static ImmutableArray<Type> EventTypes =>
    [
        typeof(FamilyEstablished),
            typeof(FamilyJoinLodge),
            typeof(FamilyLeftLodge)
    ];
    public record FamilyEstablished(
        Guid Id,
        string Name,
        string Motto,
        DateTimeOffset EstablishedAt);

    public record FamilyJoinLodge(
        Guid LodgeId);

    public record FamilyLeftLodge(
        Guid LodgeId);

    public record FamilyCritterJoined(
        Guid FamilyId,
        Guid CritterId,
        DateTimeOffset JoinedAt);

}