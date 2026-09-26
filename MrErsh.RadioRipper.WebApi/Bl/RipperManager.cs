using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MrErsh.RadioRipper.Core;
using MrErsh.RadioRipper.Dal;
using MrErsh.RadioRipper.Model;
using MrErsh.RadioRipper.WebApi.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Config = MrErsh.RadioRipper.WebApi.Configuration;

namespace MrErsh.RadioRipper.WebApi.Bl
{
    public sealed class RipperManager : IRipperManager
    {
        private record IdUrl(Guid Id, string Url);

        private readonly IDbContextFactory<RadioDbContext> _dbContextFactory;
        private readonly IRipperFactory _ripperFactory;
        private readonly ILogger<RipperManager> _logger;
        private readonly IOptionsMonitor<Config.Ripper> _ripperConfigMonitor;

        private readonly ConcurrentDictionary<Guid, TimeredRadioRipper> _running = new();
        private readonly ConcurrentDictionary<Guid, SemaphoreSlim> _stationLocks = new();

        public RipperManager(IDbContextFactory<RadioDbContext> dbContextFactory,
                             IRipperFactory ripperFactory,
                             ILogger<RipperManager> logger,
                             IOptionsMonitor<Config.Ripper> ripperConfigMonitor)
        {
            _dbContextFactory = dbContextFactory;
            _ripperFactory = ripperFactory;
            _logger = logger;
            _ripperConfigMonitor = ripperConfigMonitor;
        }

        public async Task RestartAllAsync()
        {
            _running.Clear();

            using var context = _dbContextFactory.CreateDbContext();
            {
                var stations = await context.Stations
                    .Where(st => st.IsRunning)
                    .AsNoTracking()
                    .ToListAsync()
                    .ConfigureAwait(false);

                var startTasks = stations
                    .Select(st => Task.Run(() => TryStart(st)))
                    .ToArray();

                await Task.WhenAll(startTasks);
                await context.SaveChangesAsync();
            }
        }

        public async Task<bool> RunAsync(Guid idStation)
        {
            var gate = Gate(idStation);
            await gate.WaitAsync().ConfigureAwait(false);
            try
            {
                await using var dbContext = _dbContextFactory.CreateDbContext();
                var station = await dbContext.Stations.FindAsync(idStation).ConfigureAwait(false);
                if (station == null)
                    return false;

                if (station.IsRunning && _running.ContainsKey(idStation))
                    return true;

                await using var tx = await dbContext.Database.BeginTransactionAsync().ConfigureAwait(false);
                station.IsRunning = true;
                await dbContext.SaveChangesAsync().ConfigureAwait(false);
                await tx.CommitAsync().ConfigureAwait(false);

                if (!TryStart(station))
                {
                    station.IsRunning = false;
                    await dbContext.SaveChangesAsync().ConfigureAwait(false);
                    return false;
                }

                return true;
            }
            finally
            {
                gate.Release();
            }
        }

        public async Task<bool> StopAsync(Guid idStation)
        {
            var gate = Gate(idStation);
            await gate.WaitAsync().ConfigureAwait(false);
            try
            {
                await using var dbContext = _dbContextFactory.CreateDbContext();
                var station = await dbContext.Stations.FindAsync(idStation).ConfigureAwait(false);
                if (station == null)
                    return false;

                await using var tx = await dbContext.Database.BeginTransactionAsync().ConfigureAwait(false);
                station.IsRunning = false;
                await dbContext.SaveChangesAsync().ConfigureAwait(false);
                await tx.CommitAsync().ConfigureAwait(false);

                if (_running.TryGetValue(idStation, out var ripper))
                {
                    ripper.TrackChangedAsync = null;
                    ripper.Stop();
                    _running.TryRemove(idStation, out _);
                }

                return true;
            }
            finally
            {
                gate.Release();
            }
        }

        public async Task ReconcileAsync()
        {
            List<Station> running;
            await using (var readContext = _dbContextFactory.CreateDbContext())
            {
                running = await readContext.Stations
                    .AsNoTracking()
                    .Where(s => s.IsRunning)
                    .ToListAsync()
                    .ConfigureAwait(false);
            }

            var runningIds = running.Select(s => s.Id).ToHashSet();

            foreach (var station in running)
            {
                var gate = Gate(station.Id);
                await gate.WaitAsync().ConfigureAwait(false);
                try
                {
                    if (_running.ContainsKey(station.Id))
                        continue;

                    if (TryStart(station))
                        continue;

                    await using var dbContext = _dbContextFactory.CreateDbContext();
                    var tracked = await dbContext.Stations.FindAsync(station.Id).ConfigureAwait(false);
                    if (tracked == null)
                        continue;

                    tracked.IsRunning = false;
                    await dbContext.SaveChangesAsync().ConfigureAwait(false);
                }
                finally
                {
                    gate.Release();
                }
            }

            foreach (var id in _running.Keys.ToList())
            {
                if (runningIds.Contains(id))
                    continue;

                var gate = Gate(id);
                await gate.WaitAsync().ConfigureAwait(false);
                try
                {
                    if (_running.TryGetValue(id, out var ripper))
                    {
                        ripper.TrackChangedAsync = null;
                        ripper.Stop();
                        _running.TryRemove(id, out _);
                    }
                }
                finally
                {
                    gate.Release();
                }
            }
        }

        private SemaphoreSlim Gate(Guid stationId) => _stationLocks.GetOrAdd(stationId, _ => new SemaphoreSlim(1, 1));

        private bool TryStart([NotNull] Station station)
        {
            try
            {
                var timeredRipper = _ripperFactory.CreateTimered(station);
                timeredRipper.TrackChangedAsync = OnRipperTrackChangedAsync;
                _running[station.Id] = timeredRipper;
                timeredRipper.Run(GetSettings());
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(Events.Errors.StartTrackingError, ex,
                                 "Start tracking {stationId} failed: {stationUrl}",
                                 station.Id,
                                 station.Url);
                return false;
            }
        }

        private async Task OnRipperTrackChangedAsync(TrackChangedEventArg e)
        {
            if (string.IsNullOrWhiteSpace(e.Info.StreamTitle))
            {
                _logger.LogWarning(Events.Errors.ParseError,
                                   "Error track title parsing for station={stationId}: origin={origin}.",
                                   e.StationId,
                                   e.Info.Origin);
                return;
            }

            using (var dbContext = _dbContextFactory.CreateDbContext())
            {
                var info = e.Info;
                var track = new Track
                {
                    FullName = info.StreamTitle,
                    MetadataHeader = info.Origin,
                    StationId = e.StationId,
                    Created = DateTime.UtcNow,
                    TrackName = info.TrackName,
                    Artist = info.Artist
                };

                dbContext.Tracks.Add(track);
                await dbContext.SaveChangesAsync().ConfigureAwait(false);
            }
        }

        private RipperSettings GetSettings()
        {
            var conf = _ripperConfigMonitor.CurrentValue;
            return new RipperSettings(conf.Interval, conf.NumOfAttempts);
        }
    }
}
