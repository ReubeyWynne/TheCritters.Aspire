using System.Runtime.CompilerServices;
using Marten;
using Marten.Events;
using TheCritters.Aspire.Application.Access.Commands;
using TheCritters.Aspire.Domain.Access;
using TheCritters.Aspire.Domain.Aggregates;
using TheCritters.Aspire.Infrastructure.Projections;
using static TheCritters.Aspire.Domain.Aggregates.Critter;

namespace TheCritters.Aspire.Application.Critters.Commands;

public record LeaveGuildCommand(
    Guid CritterId,
    Guid GuildId);

public static class LeaveGuildCommandAggregateHandler
{
    public static async IAsyncEnumerable<RevokeAccessCommand> Handle(
        LeaveGuildCommand command,
        IEventStream<Critter> stream,
        IDocumentSession session,
        [EnumeratorCancellation] CancellationToken ct)
    {
        var joinedEvent = new CritterLeftGuild(command.CritterId, command.GuildId, DateTime.UtcNow);
        stream.AppendOne(joinedEvent);

        var accesses = await session
            .Query<AccessDetails>()
            .Where(a => a.CritterId == command.CritterId &&
                        a.AccessSource == AuthorisationSourceType.Guild &&
                        a.SourceId == command.GuildId &&
                        a.IsActive)
            .ToListAsync(ct);

        // Revoke each access
        foreach (var access in accesses)
        {
            yield return (new RevokeAccessCommand(
                access.Id,
                $"Left guild {command.GuildId}"));
        }
    }
}
