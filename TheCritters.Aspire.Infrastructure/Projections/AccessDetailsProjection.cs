

using Marten.Events.Aggregation;
using TheCritters.Aspire.Domain.Access;
using static TheCritters.Aspire.Domain.Access.LodgeAccess.Events;

namespace TheCritters.Aspire.Infrastructure.Projections;

public class AccessDetails
{
    public Guid Id { get; set; }
    public Guid CritterId { get; set; }
    public Guid LodgeId { get; set; }
    public AuthorisationSourceType AccessSource { get; set; }
    public Guid SourceId { get; set; }
    public DateTimeOffset GrantedAt { get; set; }
    public DateTimeOffset? RevokedAt { get; set; }
    public bool IsActive => RevokedAt == null;
}

public class AccessDetailsProjection : SingleStreamProjection<AccessDetails>
{
    public static AccessDetails Create(AccessGranted @event)
    {
        return new AccessDetails
        {
            CritterId = @event.CritterId,
            LodgeId = @event.LodgeId,
            AccessSource = @event.AuthorisationSource,
            SourceId = @event.SourceId,
            GrantedAt = @event.GrantedAt
        };
    }

    public static void Apply(AccessRevoked @event, AccessDetails view)
    {
        view.RevokedAt = @event.RevokedAt;
    }
}