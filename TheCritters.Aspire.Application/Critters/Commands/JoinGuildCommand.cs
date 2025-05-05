using Marten.Events;
using TheCritters.Aspire.Domain.Aggregates;
using static TheCritters.Aspire.Domain.Aggregates.Critter.Events;

namespace TheCritters.Aspire.Application.Critters.Commands;

public record JoinGuildCommand(
    Guid CritterId,
    Guid GuildId);

public static class JoinGuildCommandAggregateHandler
{
    //[AggregateHandler]
    ///Suffixing the name of the handler clas *should* have the same effect
    ///As the attribute above
    public static void Handle(
             JoinGuildCommand command,
             IEventStream<Critter> stream) => 
        stream.AppendOne(new JoinedGuild(command.CritterId, command.GuildId, DateTime.UtcNow));
}
