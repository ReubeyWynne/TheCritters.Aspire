using Marten.Events;
using TheCritters.Aspire.Domain.Aggregates;
using static TheCritters.Aspire.Domain.Aggregates.Lodge.Events;

namespace TheCritters.Aspire.Application.Lodges.Commands;

public record CloseLodgeCommand(Guid LodgeId, DateTime Timestamp);

public static class CloseLodgeCommandAggregateHandler
{
    public static void Handle(
           CloseLodgeCommand command,
           IEventStream<Lodge> stream) =>
      stream.AppendOne(new Closed(command.LodgeId, command.Timestamp));
}