using Marten.Events.Aggregation;
using static TheCritters.Aspire.Domain.Aggregates.Lodge.Events;

namespace TheCritters.Aspire.Infrastructure.Projections;

public class LodgeOccupancy
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public int CurrentOccupancy { get; set; }
    public int Capacity { get; set; }
    public List<Guid> CurrentOccupants { get; set; } = [];
}

public class LodgeOccupancyProjection : SingleStreamProjection<LodgeOccupancy>
{
    public static LodgeOccupancy Create(Created @event)
    {
        return new LodgeOccupancy
        {
            Name = @event.Name,
            Location = @event.Location,
            Capacity = @event.Capacity,
            CurrentOccupancy = 0
        };
    }

    public static void Apply(CritterEntered @event, LodgeOccupancy view)
    {
        view.CurrentOccupancy++;
        view.CurrentOccupants.Add(@event.CritterId);
    }

    public static void Apply(CritterExited @event, LodgeOccupancy view)
    {
        view.CurrentOccupancy = Math.Max(0, view.CurrentOccupancy - 1);
        view.CurrentOccupants.Remove(@event.CritterId);
    }
}