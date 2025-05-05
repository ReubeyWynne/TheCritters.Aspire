using Marten.Events;
using TheCritters.Aspire.Domain.Aggregates;
using static TheCritters.Aspire.Domain.Aggregates.Family.Events;

namespace TheCritters.Aspire.Application.Family.Commands;

public record JoinLodgeCommand(Guid LodgeId);
public static class JoinLodgeCommandAggregateHandler
{
    public static void Handle(
           JoinLodgeCommand command,
           IEventStream<Lodge> stream) =>
      stream.AppendOne(new JoinedLodge(command.LodgeId));
}