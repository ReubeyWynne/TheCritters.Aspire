using Marten.Events;
using Marten;
using TheCritters.Aspire.Application.Access.Commands;
using TheCritters.Aspire.Domain.Aggregates;
using TheCritters.Aspire.Infrastructure.Projections;
using System.Runtime.CompilerServices;
using static TheCritters.Aspire.Domain.Aggregates.Guild;
using Wolverine;

namespace TheCritters.Aspire.Application.Guilds.Commands;

public record UnsubscribeLodgeCommand(
    Guid GuildId,
    Guid LodgeId,
    string Reason = "");

public static class UnsubscribeLodgeAggregateHandler
{
    public static async IAsyncEnumerable<RevokeAccessCommand> Handle(
        UnsubscribeLodgeCommand command,
        IEventStream<Guild> stream,
        IDocumentSession session,
        [EnumeratorCancellation] CancellationToken ct)
    {
        stream.AppendOne(new GuildLeftLodge(command.GuildId, command.LodgeId, DateTime.UtcNow));

        var guildMembers = await session
            .Query<CritterDetails>()
            .Where(c => c.GuildIds.Contains(command.GuildId))
            .ToListAsync(ct);

        var memberIds = guildMembers.Select(x => x.Id).ToArray();
        var accessRights = await session
            .Query<AccessDetails>()
            .Where(c => c.LodgeId == command.LodgeId && memberIds.Contains(c.CritterId) && c.IsActive)
            .ToListAsync(token: ct);

        foreach (var right in accessRights)
        {
            yield return new RevokeAccessCommand(
                right.Id,
                "Guild unsubscribed from Lodge")
        }
    }
}