using TheCritters.Aspire.Domain.Aggregates;
using Wolverine.Marten;
using static TheCritters.Aspire.Domain.Aggregates.Lodge.Events;

namespace TheCritters.Aspire.Application.Lodges.Commands;

public record CreateLodgeCommand(
    string Name,
    string Location,
    int Capacity,
    DateTime Timestamp);

public static class CreateLodgeHandler
{
    public static IStartStream Handle(
        CreateLodgeCommand command) => MartenOps.StartStream<Lodge>(new Created(
            Guid.NewGuid(), command.Name, command.Location, command.Capacity, command.Timestamp));
}