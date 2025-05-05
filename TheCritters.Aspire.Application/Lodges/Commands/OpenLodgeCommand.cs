using Marten.Events;
using TheCritters.Aspire.Domain.Aggregates;
using static TheCritters.Aspire.Domain.Aggregates.Lodge;

namespace TheCritters.Aspire.Application.Lodges.Commands;

public record OpenLodgeCommand(Guid LodgeId, DateTime Timestamp);

public static class OpenLodgeCommandAggregateHandler
{
    public static void Handle(
           OpenLodgeCommand command,
           IEventStream<Lodge> stream) =>
      stream.AppendOne(new LodgeOpened(command.LodgeId,  command.Timestamp));
}