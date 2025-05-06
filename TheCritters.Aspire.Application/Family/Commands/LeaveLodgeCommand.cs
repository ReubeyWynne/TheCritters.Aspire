using Marten.Events;
using static TheCritters.Aspire.Domain.Aggregates.Family;

namespace TheCritters.Aspire.Application.Family.Commands;

public record LeaveLodgeCommand(Guid FamilyId, Guid LodgeId);
public static class LeaveLodgeCommandAggregateHandler
{
    public static void Handle(
           LeaveLodgeCommand command,
           IEventStream<Domain.Aggregates.Family> stream) =>
      stream.AppendOne(new FamilyLeftLodge(command.LodgeId));
}