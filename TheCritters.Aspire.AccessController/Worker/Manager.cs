// TheCritters.Aspire.AccessController/Services/WorkerManager.cs
using System.Collections.Concurrent;
using Marten;
using TheCritters.Aspire.Infrastructure.Projections;

namespace TheCritters.Aspire.AccessController.Services
{
    public class WorkerManager : IDisposable
    {
        private readonly ILogger<WorkerManager> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IDocumentStore _documentStore;
        private readonly ConcurrentDictionary<Guid, IHostedService> _workers;

        public WorkerManager(
            ILogger<WorkerManager> logger,
            IServiceProvider serviceProvider,
            IDocumentStore documentStore)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _documentStore = documentStore;
            _workers = new ConcurrentDictionary<Guid, IHostedService>();
        }

        public async Task<bool> StartWorkerForLodge(Guid lodgeId)
        {
            if (_workers.ContainsKey(lodgeId))
            {
                _logger.LogWarning("Worker for Lodge {LodgeId} is already running", lodgeId);
                return false;
            }

            try
            {
                // Validate that the lodge exists
                using (var session = _documentStore.QuerySession())
                {
                    var lodge = await session.LoadAsync<LodgeOccupancy>(lodgeId);
                    if (lodge == null)
                    {
                        _logger.LogWarning("Attempted to start worker for non-existent Lodge {LodgeId}", lodgeId);
                        return false;
                    }
                }

                // Create the worker
                var worker = ActivatorUtilities.CreateInstance<AccessControlWorker>(
                    _serviceProvider,
                    lodgeId);

                // Start the worker
                await worker.StartAsync(CancellationToken.None);

                // Add to our tracking dictionary
                if (!_workers.TryAdd(lodgeId, worker))
                {
                    // Another thread beat us to it, stop our worker
                    await worker.StopAsync(CancellationToken.None);
                    _logger.LogWarning("Worker for Lodge {LodgeId} was already started by another thread", lodgeId);
                    return false;
                }

                _logger.LogInformation("Started worker for Lodge {LodgeId}", lodgeId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to start worker for Lodge {LodgeId}", lodgeId);
                return false;
            }
        }

        public async Task<bool> StopWorkerForLodge(Guid lodgeId)
        {
            if (!_workers.TryRemove(lodgeId, out var worker))
            {
                _logger.LogWarning("No worker found for Lodge {LodgeId}", lodgeId);
                return false;
            }

            try
            {
                // Stop the worker
                await worker.StopAsync(CancellationToken.None);
                _logger.LogInformation("Stopped worker for Lodge {LodgeId}", lodgeId);

                // If the worker implements IDisposable, dispose it
                if (worker is IDisposable disposable)
                {
                    disposable.Dispose();
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error stopping worker for Lodge {LodgeId}", lodgeId);
                return false;
            }
        }

        public async Task StopAllWorkers()
        {
            _logger.LogInformation("Stopping all workers...");

            foreach (var lodgeId in _workers.Keys)
            {
                await StopWorkerForLodge(lodgeId);
            }

            _logger.LogInformation("All workers stopped");
        }

        public void Dispose()
        {
            // Force synchronous stop of all workers
            Task.Run(async () => await StopAllWorkers()).GetAwaiter().GetResult();
            GC.SuppressFinalize(this);
        }
    }
}