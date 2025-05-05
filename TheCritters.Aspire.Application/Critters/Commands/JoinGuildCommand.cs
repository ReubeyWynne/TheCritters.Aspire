using System.Runtime.CompilerServices;
using Marten;
using Marten.Events;
using TheCritters.Aspire.Application.Access.Commands;
using TheCritters.Aspire.Domain.Access;
using TheCritters.Aspire.Domain.Aggregates;
using static TheCritters.Aspire.Domain.Aggregates.Critter;

namespace TheCritters.Aspire.Application.Critters.Commands;

public record JoinGuildCommand(
    Guid CritterId,
    Guid GuildId);

public static class JoinGuildCommandHandler
{
    public static async IAsyncEnumerable<GrantAccessCommand> Handle(
        JoinGuildCommand command,
        IEventStream<Critter> stream,
        IDocumentSession session,
        [EnumeratorCancellation] CancellationToken ct)
    {
        var joinedEvent = new CritterJoinedGuild(command.CritterId, command.GuildId, DateTime.UtcNow);
        stream.AppendOne(joinedEvent);

        var guild = await session.LoadAsync<Guild>(command.GuildId, ct);

        if (guild != null && guild.AllowedLodges.Count > 0)
        {
            foreach (var lodgeId in guild.AllowedLodges)
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