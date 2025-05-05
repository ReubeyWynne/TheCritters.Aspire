using System.Collections.Immutable;

namespace TheCritters.Aspire.Domain.Aggregates;


public class Lodge
{
    #region Serialisation Constructor
    protected Lodge() { }
    #endregion

    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Name { get; private set; } = null!;
    public string Location { get; private set; } = null!;
    public int Capacity { get; private set; }
    public int CurrentOccupancy { get; private set; }
    public bool IsOpen { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }

    public static Lodge Create(string Name, string Location) => new()
    {
        Name = Name,
        Location = Location
    };

    public void Apply(LodgeCreated @event)
    {
        Name = @event.Name;
        Location = @event.Location;
        Capacity = @event.Capacity;
        CreatedAt = @event.CreatedAt;
        IsOpen = true;
    }

    public void Apply(LodgeCritterEntered @event) => CurrentOccupancy++;

    public void Apply(LodgeCritterExited @event) =>
        CurrentOccupancy = Math.Max(0, CurrentOccupancy - 1);

    public void Apply(LodgeOpened @event) => IsOpen = true;

    public void Apply(LodgeClosed @event) => IsOpen = false;

    public static ImmutableArray<Type> EventTypes =>
         [
             typeof(LodgeCreated),
            typeof(LodgeCritterEntered),
            typeof(LodgeCritterExited),
            typeof(LodgeOpened),
            typeof(LodgeClosed)
         ];

    public record LodgeCreated(
        Guid Id,
        string Name,
        string Location,
        int Capacity,
        DateTimeOffset CreatedAt);


    public record LodgeCritterEntered(
        Guid LodgeId,
        Guid CritterId,
        DateTimeOffset EnteredAt);

    public record LodgeCritterExited(
        Guid LodgeId,
        Guid CritterId,
        DateTimeOffset ExitedAt);

    public record LodgeOpened(
        Guid LodgeId,
        DateTimeOffset OpenedAt);

    public record LodgeClosed(
        Guid LodgeId,
        DateTimeOffset ClosedAt);




}