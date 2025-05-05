# CritterStack

CritterStack is a .NET 9 event sourcing application built with .NET Aspire that demonstrates using Marten and Wolverine to implement a domain-driven design system.

## Overview

CritterStack demonstrates event sourcing principles and domain modeling using small, focused aggregates. The system models a fictional world of "Critters" who can join Guilds and reside in Lodges, with Families representing groups of related critters.

### Architecture

The application follows a clean architecture approach with the following components:

- **Domain Layer**: Contains the core business objects (aggregates, entities, value objects)
- **Application Layer**: Contains application services, commands and command handlers
- **Infrastructure Layer**: Implements persistence and other infrastructure concerns
- **Web UI**: Blazor-based UI for interacting with the system
- **AccessController**: Separate service for handling access control rules

## Key Technologies

- **.NET 9**: The latest version of the .NET framework
- **.NET Aspire**: For cloud-ready distributed application development
- **Marten**: Document database and event store for PostgreSQL
- **Wolverine**: Message bus and command handling
- **Blazor**: Web UI framework
- **MudBlazor**: Material Design components for Blazor
- **PostgreSQL**: Database engine for persistence

## Domain Model

### Aggregates

The system is built around the following aggregates:

- **Critter**: Represents an individual critter
- **Guild**: Organizations that critters can join
- **Lodge**: Physical locations where critters can stay
- **Family**: Groups of related critters
- **AccessAuth**: Controls access permissions to lodges

### Event Sourcing

All state changes in the system are modeled as events:

- Critter events: `Registered`, `JoinedGuild`, `LeftGuild`
- Guild events: `Created`, `AccessGranted`, `AccessRevoked`
- Lodge events: `Created`, `CritterEntered`, `CritterExited`, `Opened`, `Closed`
- Family events: `Established`, `JoinedLodge`, `LeftLodge`, `CritterJoined`
- Access events: `AccessGranted`, `AccessRevoked`, `AccessAttempted`

## Getting Started

### Prerequisites

- .NET 9 SDK
- PostgreSQL 16+
- Docker (optional)

### Setup

1. Clone the repository
2. Ensure PostgreSQL is running (or use the included Docker configuration)
3. Build the solution:dotnet build
4. Run the app:
dotnet run --project TheCritters.Aspire.AppHost

This will start the Aspire dashboard and the application services.

### Exploring the Application

Once running, you can:

1. Visit the Blazor UI to interact with the system
2. Register new critters
3. Create guilds and lodges
4. Assign critters to guilds
5. View projections of the data

## Architecture Details

### Command Flow

Commands in the system follow this general flow:

1. UI sends command to the application layer
2. Command handler processes the command
3. Events are created and appended to the event stream
4. Projections process events to update read models
5. UI displays updated read models

### Projections

The system includes several projections to provide optimized read models:

- `CritterDetailsProjection`: Details about critters and their guild memberships
- `GuildProjection`: Information about guilds and allowed lodges
- `LodgeOccupancyProjection`: Tracking which critters are in which lodges
- `FamilyDetailsProjection`: Information about families

## Testing

To run the tests:
dotnet test

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## License

This project is licensed under the MIT License - see the LICENSE file for details.