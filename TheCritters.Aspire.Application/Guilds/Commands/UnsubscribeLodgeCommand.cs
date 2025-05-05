using Marten.Events;
using Marten;
using TheCritters.Aspire.Application.Access.Commands;
using TheCritters.Aspire.Domain.Aggregates;
using TheCritters.Aspire.Infrastructure.Projections;
using System.Runtime.CompilerServices;
using static TheCritters.Aspire.Domain.Aggregates.Guild;

namespace TheCritters.Aspire.Application.Guilds.Commands;

public record UnsubscribeLodgeCommand(
    Guid GuildId,
    Guid LodgeId);

public static class UnsubscribeLodgeAggregateHandler
{
    public static async IAsyncEnumerable<GrantAccessCommand> Handle(
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


        foreach (var critter in guildMembers)
        {
            yield return new GrantAccessCommand(
                critter.Id,
                command.LodgeId,
                Domain.Access.AuthorisationSourceType.Guild,
                command.GuildId);
        }
    }
}