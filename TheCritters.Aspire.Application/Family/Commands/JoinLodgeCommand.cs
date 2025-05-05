using Marten.Events;
using TheCritters.Aspire.Domain.Aggregates;
using static TheCritters.Aspire.Domain.Aggregates.Family;

namespace TheCritters.Aspire.Application.Family.Commands;

public record JoinLodgeCommand(Guid LodgeId);
public static class JoinLodgeCommandAggregateHandler
{
    public static void Handle(
           JoinLodgeCommand command,
           IEventStream<Lodge> stream) =>
      stream.AppendOne(new FamilyJoinLodge(command.LodgeId));
}