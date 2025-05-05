using Marten.Events;
using TheCritters.Aspire.Domain.Access;

namespace TheCritters.Aspire.Application.Access.Commands;

public record RevokeAccessCommand(
    Guid LodgeAccessId,
    string Reason);

public static class RevokeAccessCommandAggregateHandler
{
    public static void Handle(
        RevokeAccessCommand command,
        IEventStream<LodgeAccess> stream)
    {
        var accessRevoked = stream.Aggregate.Revoke(command.Reason);
        stream.AppendOne(accessRevoked);
    }
}