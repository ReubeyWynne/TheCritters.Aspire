using Marten.Events.Aggregation;
using static TheCritters.Aspire.Domain.Aggregates.Family.Events;

namespace TheCritters.Aspire.Infrastructure.Projections;

public class FamilyDetails
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Motto { get; set; } = string.Empty;
    public DateTimeOffset EstablishedAt { get; set; }
    public bool IsActive { get; set; }
}

public class FamilyDetailsProjection : SingleStreamProjection<FamilyDetails>
{
    public static FamilyDetails Create(Established @event)
    {
        return new FamilyDetails
        {
            Name = @event.Name,
            Motto = @event.Motto,
            EstablishedAt = @event.EstablishedAt,
        };
    }

}