using System.Runtime.CompilerServices;
using Marten;
using Marten.Events;
using TheCritters.Aspire.Application.Access.Commands;
using TheCritters.Aspire.Domain.Access;
using TheCritters.Aspire.Domain.Aggregates;
using TheCritters.Aspire.Infrastructure.Projections;
using static TheCritters.Aspire.Domain.Aggregates.Critter;

namespace TheCritters.Aspire.Application.Critters.Commands;

public record JoinGuildCommand(
    Guid CritterId,
    Guid GuildId);

public static class JoinGuildCommandAggregateHandler
{
    public static async IAsyncEnumerable<GrantAccessCommand> Handle(
        JoinGuildCommand command,
        IEventStream<Critter> stream,
        IDocumentSession session,
        [EnumeratorCancellation] CancellationToken ct)
    {
        var joinedEvent = new CritterJoinedGuild(command.CritterId, command.GuildId, DateTime.UtcNow);
        stream.AppendOne(joinedEvent);

        var guild = await session.LoadAsync<GuildDetails>(command.GuildId, ct);

        if (guild != null && guild.SubscribedLodges.Count > 0)
        {
            foreach (var lodgeId in guild.SubscribedLodges)
            {
                yield return new GrantAccessCommand(
                    command.CritterId,
                    lodgeId,
                    AuthorisationSourceType.Guild,
                    command.GuildId);
            }
        }
    }
}