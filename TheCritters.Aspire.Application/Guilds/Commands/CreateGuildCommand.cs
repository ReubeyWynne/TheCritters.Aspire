using TheCritters.Aspire.Domain.Aggregates;
using Wolverine.Marten;
using static TheCritters.Aspire.Domain.Aggregates.Guild;

namespace TheCritters.Aspire.Application.Guilds.Commands;

public class CreateGuildCommand(
    string Name,
    string Description)
{
    public string Name { get; set; } = Name;
    public string Description { get; set; } = Description;
}

public static class CreateGuildCommandHandler
{
    public static IStartStream Handle(
        CreateGuildCommand cmd)
        => MartenOps.StartStream<Guild>(
            new GuildCreated(
                Guid.NewGuid(), cmd.Name, cmd.Description, DateTime.UtcNow));
}