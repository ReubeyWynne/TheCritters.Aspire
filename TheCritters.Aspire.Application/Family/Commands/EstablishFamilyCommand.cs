using Wolverine.Marten;
using static TheCritters.Aspire.Domain.Aggregates.Family;

namespace TheCritters.Aspire.Application.Family.Commands;

public record EstablishFamilyCommand(
    string Name,
    string Motto,
    DateTime Timestamp);

public static class EstablishFamilyHandler
{
    public static IStartStream Handle(
        EstablishFamilyCommand command) => MartenOps.StartStream<Domain.Aggregates.Family>(new FamilyEstablished(
            Guid.NewGuid(), command.Name, command.Motto, command.Timestamp));
}