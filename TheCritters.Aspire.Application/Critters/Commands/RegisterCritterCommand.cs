// src/TheCritters.Aspire.Application/Critters/Commands/RegisterCritterCommand.cs
using Wolverine.Attributes;
using Wolverine.Marten;
using static TheCritters.Aspire.Domain.Aggregates.Critter;
using TheCritters.Aspire.Domain.Aggregates;

namespace TheCritters.Aspire.Application.Critters.Commands;

public class RegisterCritterCommand(
    string Name,
    string Species)
{
    public string Name { get; set; } = Name;
    public string Species { get; set; } = Species;
    public DateTime RegistedAt { get; set; } = DateTime.UtcNow;
}

[WolverineHandler]
public static class RegisterCritterCommandHandler
{
    public static IStartStream Handle(
        RegisterCritterCommand cmd) => 
            MartenOps.StartStream<Critter>(
                new CritterRegistered(
                    Guid.NewGuid(), cmd.Name, cmd.Species, DateTime.UtcNow));
}