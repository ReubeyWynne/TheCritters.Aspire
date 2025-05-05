using System.Collections.Immutable;
using static TheCritters.Aspire.Domain.Aggregates.Lodge.Events;

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

    public void Apply(Created @event)
    {
        Name = @event.Name;
        Location = @event.Location;
        Capacity = @event.Capacity;
        CreatedAt = @event.CreatedAt;
        IsOpen = true;
    }

    public void Apply(CritterEntered @event) => CurrentOccupancy++;

    public void Apply(CritterExited @event) =>
        CurrentOccupancy = Math.Max(0, CurrentOccupancy - 1);

    public void Apply(Opened @event) => IsOpen = true;

    public void Apply(Closed @event) => IsOpen = false;

    public static class Events
    {
        public static ImmutableArray<Type> Types =>
        [
            typeof(Created),
            typeof(CritterEntered),
            typeof(CritterExited),
            typeof(Opened),
            typeof(Closed)
        ];

        public record Created(
            Guid Id,
            string Name,
            string Location,
            int Capacity,
            DateTimeOffset CreatedAt);


        public record CritterEntered(
            Guid LodgeId,
            Guid CritterId,
            DateTimeOffset EnteredAt);

        public record CritterExited(
            Guid LodgeId,
            Guid CritterId,
            DateTimeOffset ExitedAt);

        public record Opened(
            Guid LodgeId,
            DateTimeOffset OpenedAt);

        public record Closed(
            Guid LodgeId,
            DateTimeOffset ClosedAt);
    }




}