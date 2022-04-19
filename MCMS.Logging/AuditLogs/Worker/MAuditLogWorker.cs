using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MCMS.Base.Auth;
using MCMS.Base.Extensions;
using MCMS.Logging.AuditLogs.AuditLogEntries;
using MCMS.Logging.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace MCMS.Logging.AuditLogs.Worker
{
    internal class MAuditLogWorker
    {
        private readonly ILogger _logger;
        private readonly IServiceProvider _serviceProvider;

        private readonly ConcurrentQueue<LogActionWrapper<AuditLogEntryEntity>> _pendingEntries = new();

        private readonly SemaphoreSlim _semaphore = new(0, 1);

        private readonly CancellationTokenSource _cancellationTokenSource = new();

        private const int BatchCount = 50;
        private bool _working;

        public MAuditLogWorker(
            ILoggerFactory loggerFactory,
            IServiceProvider serviceProvider)
        {
            _logger = loggerFactory.CreateLogger("MAuditLogWorker");
            _serviceProvider = serviceProvider.CreateScope().ServiceProvider;
        }

        private async Task Work(CancellationToken token)
        {
            _logger.LogInformation("Work method: Started");
            while (true)
            {
                _logger.LogInformation("Work method: waiting for semaphore");
                await _semaphore.WaitAsync(token);
                _logger.LogInformation("Work method: Got green semaphore");
                await Task.Delay(500, token);
                try
                {
                    do
                    {
                        var entries = _pendingEntries.Dequeue(BatchCount);
                        if (entries.Count == 0)
                        {
                            _logger.LogWarning("Got 0 entries from pending queue");
                            continue;
                        }

                        _logger.LogInformation("Got {Count} entries from pending queue", entries.Count);
                        var repo = _serviceProvider.GetRepo<AuditLogEntryEntity>();

                        var toAddEntries = entries.Where(e => e.Type == ActionType.Add).Select(w => w.Log).ToList();

                        if (toAddEntries.Any())
                        {
                            _logger.LogInformation("Adding entries");
                            await AddEntries(toAddEntries, repo.DbSet, token);
                        }

                        var toUpdateEntries =
                            entries.Where(e => e.Type == ActionType.Update).Select(w => w.Log).ToList();

                        if (toUpdateEntries.Any())
                        {
                            _logger.LogInformation("Updating entries");
                            await UpdateEntries(toUpdateEntries, repo.DbSet, token, toAddEntries);
                        }

                        _logger.LogInformation("Saving changes");
                        await repo.SaveChanges();

                        _logger.LogInformation("Saved");
                    } while (!_pendingEntries.IsEmpty);
                }
                catch (Exception e)
                {
                    // TODO: do something with those entries
                    _logger.LogError(e, "Something very wrong occurred in MAuditLogWorker");
                }

                _working = false;
                if (token.IsCancellationRequested)
                {
                    break;
                }
            }

            _logger.LogInformation("Work method: Stopped");
        }

        private async Task AddEntries(List<AuditLogEntryEntity> entries, DbSet<AuditLogEntryEntity> dbSet,
            CancellationToken token)
        {
            var usersRepo = _serviceProvider.GetRepo<User>();
            entries.ForEach(e =>
                e.Author = e.Author?.Id != null ? usersRepo.Attach(e.Author?.Id) : null);
            await dbSet.AddRangeAsync(entries, token);
        }

        private async Task UpdateEntries(List<AuditLogEntryEntity> entries, DbSet<AuditLogEntryEntity> dbSet,
            CancellationToken token, List<AuditLogEntryEntity> toAddEntries)
        {
            var toUpdateFromDb = new List<AuditLogEntryEntity>();
            foreach (var entry in entries)
            {
                var existingEntry = toAddEntries.FirstOrDefault(e => e.TraceIdentifier == entry.TraceIdentifier);
                if (existingEntry == null)
                {
                    toUpdateFromDb.Add(entry);
                    continue;
                }

                foreach (var (key, value) in entry.Data)
                {
                    existingEntry.Data[key] = value;
                }

                existingEntry.End = entry.End;
            }

            if (toUpdateFromDb.Any())
            {
                var tis = toUpdateFromDb.Select(e => e.TraceIdentifier);
                var existingEntries = await dbSet.Where(e => tis.Contains(e.TraceIdentifier)).ToListAsync(token);
                foreach (var existingEntry in existingEntries)
                {
                    var entry = toUpdateFromDb.FirstOrDefault(t => t.TraceIdentifier == existingEntry.TraceIdentifier);
                    if (entry == null)
                    {
                        throw new Exception("Got an invalid Audit log entry from db. This should never happen.");
                    }

                    foreach (var (key, value) in entry.Data)
                    {
                        existingEntry.Data[key] = value;
                    }
                    existingEntry.End = entry.End;
                }
            }
        }

        public void Start()
        {
            _logger.LogInformation("Starting MAuditLogWorker");
            Task.Run(async () => { await Work(_cancellationTokenSource.Token); }, _cancellationTokenSource.Token);
        }

        public void Stop()
        {
            _logger.LogInformation("Stopping MAuditLogWorker");
            _cancellationTokenSource.Cancel();
            _semaphore.Release();
        }

        internal void Enqueue(LogActionWrapper<AuditLogEntryEntity> entry)
        {
            _logger.LogInformation("Enqueuing an audit entry. Semaphore CurrentCount: {Count} Working: {Working}",
                _semaphore.CurrentCount, _working);
            entry.Log.Id = null;
            _pendingEntries.Enqueue(entry);
            if (!_working && _semaphore.CurrentCount == 0)
            {
                _working = true;
                _logger.LogInformation("Releasing semaphore");
                _semaphore.Release();
            }
        }
    }
}