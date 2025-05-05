using System.Collections.Immutable;
using static TheCritters.Aspire.Domain.Aggregates.Guild.Events;

namespace TheCritters.Aspire.Domain.Aggregates;

public class Guild
{

    #region Serialisation Constructor
    protected Guild() { }
    #endregion

    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Name { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public List<Guid> AllowedLodges { get; private set; } = [];
    public DateTimeOffset CreatedAt { get; private set; }

    public static Guild Create(string Name, string Description, DateTimeOffset CreatedAt) => new()
    {
        Name = Name,
        Description = Description,
        CreatedAt = CreatedAt
    };

    public void Apply(Created @event)
    {
        Name = @event.Name;
        Description = @event.Description;
        CreatedAt = @event.CreatedAt;
    }

    public void Apply(AccessGranted @event) => AllowedLodges.Add(@event.LodgeId);

    public void Apply(AccessRevoked @event) => AllowedLodges.Remove(@event.LodgeId);


    public static class Events
    {
        public static ImmutableArray<Type> Types =>
        [
                typeof(Created),
                typeof(AccessGranted),
                typeof(AccessRevoked),
          ];
        public record Created(
            Guid Id,
            string Name,
            string Description,
            DateTimeOffset CreatedAt);

        public record AccessGranted(
            Guid GuildId,
            Guid LodgeId,
            DateTimeOffset GrantedAt);

        public record AccessRevoked(
            Guid GuildId,
            Guid LodgeId,
            DateTimeOffset RevokedAt);
    }
}