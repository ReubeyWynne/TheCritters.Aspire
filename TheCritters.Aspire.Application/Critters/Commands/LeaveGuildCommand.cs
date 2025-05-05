using Marten.Events;
using TheCritters.Aspire.Domain.Aggregates;
using static TheCritters.Aspire.Domain.Aggregates.Critter.Events;

namespace TheCritters.Aspire.Application.Critters.Commands;

public record LeaveGuildCommand(
    Guid CritterId,
    Guid GuildId);

public static class LeaveGuildCommandAggregateHandler
{
    public static void Handle(
        LeaveGuildCommand command,
        IEventStream<Critter> stream) => 
            stream.AppendOne(new LeftGuild(command.CritterId,
                                                  command.GuildId,
                                                  DateTime.UtcNow));
}
