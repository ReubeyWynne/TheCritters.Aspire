using Marten.Events;
using TheCritters.Aspire.Application.Lodges.Commands;
using static TheCritters.Aspire.Domain.Aggregates.Family;

namespace TheCritters.Aspire.Application.Family.Commands;

public record LeaveLodgeCommand(Guid LodgeId);
public static class LeaveLodgeCommandAggregateHandler
{
    public static void Handle(
           CloseLodgeCommand command,
           IEventStream<Domain.Aggregates.Family> stream) =>
      stream.AppendOne(new FamilyLeftLodge(command.LodgeId));
}