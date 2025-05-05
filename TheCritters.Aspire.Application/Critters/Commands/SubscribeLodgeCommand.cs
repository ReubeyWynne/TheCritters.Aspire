using Marten.Events;
using Marten;
using TheCritters.Aspire.Application.Access.Commands;
using TheCritters.Aspire.Domain.Aggregates;
using TheCritters.Aspire.Infrastructure.Projections;
using static TheCritters.Aspire.Domain.Aggregates.Family;
using System.Runtime.CompilerServices;

namespace TheCritters.Aspire.Application.Critters.Commands;

public record SubscribeLodgeCommand(
    Guid GuildId,
    Guid LodgeId);

public static class JoinLodgeAccessAggregateHandler
{
    public static async IAsyncEnumerable<GrantAccessCommand> Handle(
        SubscribeLodgeCommand command,
        IEventStream<Guild> stream,
        IDocumentSession session,
        [EnumeratorCancellation] CancellationToken ct)
    {
        stream.AppendOne(new FamilyJoinLodge(command.LodgeId));

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