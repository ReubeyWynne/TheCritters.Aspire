using System.Collections.Immutable;

namespace TheCritters.Aspire.Domain.Access;

public enum AuthorisationSourceType
{
    Family,
    Guild,
    DirectGrant,
    TemporaryAccess
}

public class LodgeAccess
{

    #region Serialisation Constructor
    protected LodgeAccess() { }
    #endregion

    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid CritterId { get; private set; }
    public Guid LodgeId { get; private set; }
    public AuthorisationSourceType AuthorisationSource { get; private set; }
    public Guid SourceId { get; private set; } // FamilyId or GuildId if applicable
    public DateTimeOffset GrantedAt { get; private set; }
    public DateTimeOffset? RevokedAt { get; private set; }
    public bool IsActive => RevokedAt == null;

    // Factory method that creates the aggregate AND the initial event
    public static (LodgeAccess authorisation, AccessGranted @event) Grant(
        Guid critterId,
        Guid lodgeId,
        AuthorisationSourceType authorisationSource,
        Guid sourceId)
    {
        // Create the event first
        var @event = new AccessGranted(
            critterId,
            lodgeId,
            authorisationSource,
            sourceId,
            DateTimeOffset.UtcNow);

        // Create and hydrate the aggregate with the event
        var authorisation = new LodgeAccess();
        authorisation.Apply(@event);

        return (authorisation, @event);
    }

    // Method to revoke access that returns the event to be persisted
    public AccessRevoked Revoke(string reason)
    {
        if (RevokedAt.HasValue)
            throw new InvalidOperationException("Access has already been revoked");

        return new AccessRevoked(
            CritterId,
            LodgeId,
            reason,
            DateTimeOffset.UtcNow);
    }

    // Event handlers - these actually modify the aggregate state
    public void Apply(AccessGranted @event)
    {
        Id = Guid.NewGuid(); // Generate a deterministic ID based on the event data
        CritterId = @event.CritterId;
        LodgeId = @event.LodgeId;
        AuthorisationSource = @event.AuthorisationSource;
        SourceId = @event.SourceId;
        GrantedAt = @event.GrantedAt;
    }

    public void Apply(AccessRevoked @event)
    {
        RevokedAt = @event.RevokedAt;
    }


    public static ImmutableArray<Type> EventTypes =>
        [
            typeof(AccessAttempted),
            typeof(AccessRevoked),
            typeof(AccessGranted)
        ];

    public record AccessGranted(
        Guid CritterId,
        Guid LodgeId,
        AuthorisationSourceType AuthorisationSource,
        Guid SourceId,
        DateTimeOffset GrantedAt);

    public record AccessRevoked(
        Guid CritterId,
        Guid LodgeId,
        string Reason,
        DateTimeOffset RevokedAt);

    public record AccessAttempted(
        Guid CritterId,
        Guid LodgeId,
        DateTimeOffset AttemptedAt,
        bool WasSuccessful,
        string? DenialReason = null);
}


