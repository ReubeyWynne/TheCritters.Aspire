using TheCritters.Aspire.Domain.Access;
using Wolverine.Marten;
using static TheCritters.Aspire.Domain.Access.LodgeAccess;

namespace TheCritters.Aspire.Application.Access.Commands;

public record GrantAccessCommand(
    Guid CritterId,
    Guid LodgeId,
    AuthorisationSourceType AccessSource,
    Guid SourceId);

public static class GrantAccessCommandHandler
{
    public static (LodgeAccess, IStartStream) Handle(
        GrantAccessCommand command)
    {
        // Create a new access authorization
        var (access, @event) = LodgeAccess.Grant(
            command.CritterId,
            command.LodgeId,
            command.AccessSource,
            command.SourceId);

        return (access, MartenOps.StartStream<AccessGranted>(access.Id, @event));
    }
}