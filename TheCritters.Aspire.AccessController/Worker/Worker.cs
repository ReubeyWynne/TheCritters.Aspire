using Marten;
using Marten.Events;
using TheCritters.Aspire.Domain.Access;
using static TheCritters.Aspire.Domain.Access.LodgeAccess.Events;

namespace TheCritters.Aspire.AccessController.Services
{
    public class AccessControlWorker : BackgroundService
    {
        private readonly ILogger<AccessControlWorker> _logger;
        private readonly IDocumentStore _documentStore;
        private readonly Guid _lodgeId;
        private DateTimeOffset _lastCheckpoint;

        public AccessControlWorker(
            ILogger<AccessControlWorker> logger,
            IDocumentStore documentStore,
            Guid lodgeId)
        {
            _logger = logger;
            _documentStore = documentStore;
            _lodgeId = lodgeId;
            _lastCheckpoint = DateTimeOffset.UtcNow.AddDays(-1); // Start by getting events from the last day
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Starting AccessControlWorker for Lodge {LodgeId}", _lodgeId);

            try
            {
                // Initial fetch of all access authorizations for this lodge
                var initialAuths = await FetchAccessAuthsAsync();
                _logger.LogInformation("Initial fetch complete. Found {Count} access authorizations for Lodge {LodgeId}",
                    initialAuths.Count, _lodgeId);

                // Main worker loop
                while (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        // Sleep for 2 minutes
                        _logger.LogDebug("Worker sleeping for 2 minutes...");
                        await Task.Delay(TimeSpan.FromMinutes(2), stoppingToken);

                        // Check for new events since last checkpoint
                        _logger.LogDebug("Checking for new access events since {Checkpoint}", _lastCheckpoint);
                        var newEvents = await FetchNewEventsAsync(_lastCheckpoint);

                        if (newEvents.Count > 0)
                        {
                            _logger.LogInformation("Found {Count} new access events for Lodge {LodgeId}",
                                newEvents.Count, _lodgeId);
                            // Process new events
                            ProcessEvents(newEvents);
                        }
                        else
                        {
                            _logger.LogDebug("No new access events found for Lodge {LodgeId}", _lodgeId);
                        }

                        // Update checkpoint
                        _lastCheckpoint = DateTimeOffset.UtcNow;
                    }
                    catch (OperationCanceledException)
                    {
                        // Cancellation requested, break the loop
                        break;
                    }
                    catch (Exception ex)
                    {
                        // Log the error but continue the worker loop
                        _logger.LogError(ex, "Error occurred while processing access events for Lodge {LodgeId}", _lodgeId);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Fatal error in AccessControlWorker for Lodge {LodgeId}", _lodgeId);
                throw; // Let the service restart the worker
            }
            finally
            {
                _logger.LogInformation("AccessControlWorker stopping for Lodge {LodgeId}", _lodgeId);
            }
        }

        private async Task<IReadOnlyList<LodgeAccess>> FetchAccessAuthsAsync()
        {
            using var session = _documentStore.QuerySession();
            var auths = await session.Query<LodgeAccess>()
                .Where(a => a.LodgeId == _lodgeId)
                .ToListAsync();
            return auths;
        }

        private async Task<List<object>> FetchNewEventsAsync(DateTimeOffset since)
        {
            using var session = _documentStore.LightweightSession();

            // Use Marten's event store capabilities to fetch events
            var events = new List<object>();

            // Fetch AccessGranted events
            var grantedEvents = await session.Events.QueryAllRawEvents()
                .Where(e => e.EventTypesAre(typeof(AccessGranted), typeof(AccessRevoked)))
                .ToListAsync();
            events.AddRange(grantedEvents.Select(e => e.Data));

          
            return events;
        }

        private void ProcessEvents(List<object> events)
        {
            foreach (var evt in events)
            {
                switch (evt)
                {
                    case AccessGranted granted:
                        _logger.LogInformation("Access granted to Lodge {LodgeId} for Critter {CritterId}",
                            granted.LodgeId, granted.CritterId);
                        // Logic for handling access grants
                        break;

                    case AccessRevoked revoked:
                        _logger.LogInformation("Access revoked from Lodge {LodgeId} for Critter {CritterId}: {Reason}",
                            revoked.LodgeId, revoked.CritterId, revoked.Reason);
                        // Logic for handling access revocations
                        break;

                }
            }

            // Additional logic to update internal state, record stats, trigger notifications, etc.
        }
    }
}