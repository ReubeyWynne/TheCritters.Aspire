using Marten.Events;
using Marten;
using TheCritters.Aspire.Application.Access.Commands;
using TheCritters.Aspire.Domain.Aggregates;
using TheCritters.Aspire.Infrastructure.Projections;
using System.Runtime.CompilerServices;
using static TheCritters.Aspire.Domain.Aggregates.Guild;
using JasperFx.Core;
using Wolverine;

namespace TheCritters.Aspire.Application.Guilds.Commands;

public record SubscribeLodgeCommand(
    Guid GuildId,
    Guid LodgeId,
    TimeSpan? Duration = null);

public static class JoinLodgeAccessAggregateHandler
{
    public static async IAsyncEnumerable<object> Handle(
        SubscribeLodgeCommand command,
        IEventStream<Guild> stream,
        IDocumentSession session,
        [EnumeratorCancellation] CancellationToken ct)
    {
        var (GuildId, LodgeId, Duration) = command;

        stream.AppendOne(new GuildJoinedLodge(GuildId, LodgeId, DateTime.UtcNow));

        var guildMembers = await session
            .Query<CritterDetails>()
            .Where(c => c.GuildIds.Contains(GuildId))
            .ToListAsync(ct);

        foreach (var critter in guildMembers)
        {
            yield return new GrantAccessCommand(
                critter.Id,
                LodgeId,
                Domain.Access.AuthorisationSourceType.Guild,
                GuildId);
        }

        if(Duration.HasValue)
        {
            yield return new UnsubscribeLodgeCommand(
                GuildId,
                LodgeId,
                "Membership Lapsed").WithDeliveryOptions(new DeliveryOptions { ScheduleDelay = Duration});
        }
    }
}