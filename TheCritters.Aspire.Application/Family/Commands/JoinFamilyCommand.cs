using Marten.Events;
using TheCritters.Aspire.Application.Critters.Commands;
using TheCritters.Aspire.Domain.Aggregates;
using static TheCritters.Aspire.Domain.Aggregates.Family.Events;

namespace TheCritters.Aspire.Application.Family.Commands;

public record JoinFamilyCommand(
    Guid CritterId,
    Guid GuildId);

public static class JoinFamilyCommandAggregateHandler
{
    //[AggregateHandler]
    ///Suffixing the name of the handler class is the same
    ///As the attribute above
    public static void Handle(
             JoinGuildCommand command,
             IEventStream<Critter> stream) =>
        stream.AppendOne(new CritterJoined(command.CritterId, command.GuildId, DateTime.UtcNow));
}