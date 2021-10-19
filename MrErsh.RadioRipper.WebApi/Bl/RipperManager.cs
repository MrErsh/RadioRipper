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
                var startTasks = context.Stations
                    .Where(st => st.IsRunning)
                    .AsNoTracking()
                    .ToList()
                    .Select(st => Task.Run(() => TryStart(st)))
                    .ToArray();

                await Task.WhenAll(startTasks);
                await context.SaveChangesAsync();
            }
        }

        public async Task<bool> RunAsync(Guid idStation)
        {
            using var dbContext = _dbContextFactory.CreateDbContext();
            var station = await dbContext.Stations.FindAsync(idStation);
            if (station == null)
                return false;

            TryStart(station);

            station.IsRunning = true;
            dbContext.Stations.Update(station);
            await dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> StopAsync(Guid idStation)
        {
            var ripperExists = _running.TryGetValue(idStation, out var ripper);
            if (ripperExists)
            {
                ripper.Stop();
                ripper.TrackChanged -= OnRipperTrackChanged;
                _running.Remove(idStation, out var _);
            }

            using var _dbContext = _dbContextFactory.CreateDbContext();
            var station = await _dbContext.Stations.FindAsync(idStation);
            if (station == null)
                return false;

            station.IsRunning = false;
            _dbContext.Update(station);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        private bool TryStart([NotNull] Station station)
        {
            try
            {
                //var ripper = _ripperFactory.Create();
                //var title = ripper.ReadHeader(station.Url, _settings);
                //if (title?.StreamTitle == null)
                //    return false;

                var timeredRipper = _ripperFactory.CreateTimered(station);
                _running[station.Id] = timeredRipper;
                timeredRipper.Run(GetSettings());
                timeredRipper.TrackChanged += OnRipperTrackChanged;
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

        private void OnRipperTrackChanged(object sender, TrackChangedEventArg e)
        {
            if (sender is not TimeredRadioRipper timeredRipper)
                return;

            if (string.IsNullOrWhiteSpace(e.Info.StreamTitle))
            {
                _logger.LogWarning(Events.Errors.ParseError,
                                   "Error track title parsing for station={stationId}: origin={origin}.",
                                   timeredRipper.StationId,
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
                    StationId = timeredRipper.StationId,
                    Created = DateTime.UtcNow,
                    TrackName = info.TrackName,
                    Artist = info.Artist
                };

                dbContext.Tracks.Add(track);
                dbContext.SaveChanges();
            }
        }

        private RipperSettings GetSettings()
        {
            var conf = _ripperConfigMonitor.CurrentValue;
            return new RipperSettings(conf.Interval, conf.NumOfAttempts);
        }
    }
}
