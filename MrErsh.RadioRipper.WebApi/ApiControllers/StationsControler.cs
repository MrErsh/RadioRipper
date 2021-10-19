using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MrErsh.RadioRipper.Dal;
using MrErsh.RadioRipper.IdentityDal;
using MrErsh.RadioRipper.Model;
using MrErsh.RadioRipper.Model.Dto;
using MrErsh.RadioRipper.WebApi.Bl;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MrErsh.RadioRipper.WebApi.ApiControllers
{
    [Route("api/[controller]")]
    [ApiController, RequireHttps]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class StationsController : ControllerBase
    {
        private readonly IDbContextFactory<RadioDbContext> _contextFactory;
        private readonly IRipperManager _ripperManager;
        private readonly ILogger<StationsController> _logger;

        private string UserId => this.GetCurrentUserId();

        public StationsController(IDbContextFactory<RadioDbContext> contextFactory,
                                  IRipperManager ripperManager,
                                  ILogger<StationsController> logger)
        {
            _contextFactory = contextFactory;
            _ripperManager = ripperManager;
            _logger = logger;
        }

        /// <summary>
        /// Get station list for current user
        /// </summary>
        // GET: api/Stations
        [HttpGet]
        [Authorize(Permission.Stations.VIEW)]
        public async Task<ActionResult<IEnumerable<Station>>> GetStations()
        {
            using var context = _contextFactory.CreateDbContext();
            var stations = await context
                .StationsForUser(UserId)
                .AsNoTracking()
                .ToListAsync()
                .ConfigureAwait(false);

            return stations;
        }

        /// <summary>
        /// Get station info for current user
        /// </summary>
        // GET: api/Stations/5
        [HttpGet("{id}")]
        [Authorize(Permission.Stations.VIEW)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Station>> GetStation([Required] Guid id)
        {
            using var context = _contextFactory.CreateDbContext();
            var station = await context
                .StationsForUser(UserId)
                .FirstOrDefaultAsync(st => st.Id == id)
                .ConfigureAwait(false);

            return station == null
                ? NotFound()
                : Ok(station);
        }

        /// <summary>
        /// Create station
        /// </summary>
        /// <param name="station">Station item</param>
        /// <returns>Created station item</returns>
        /// <response code="201">Return created station</response>
        /// <response code="400">if station parameter is null</response>
        // POST: api/Stations
        [HttpPost]
        [Authorize(Permission.Stations.ADD)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Station>> PostStation(AddStationDto dto)
        {
            using (var context = _contextFactory.CreateDbContext())
            {
                var station = new Station { Url = dto.Url, Name = dto.Name, UserId = UserId };
                await context.Stations.AddAsync(station).ConfigureAwait(false);
                await context.SaveChangesAsync().ConfigureAwait(false);
                return CreatedAtAction(nameof(GetStation), new { id = station.Id }, station);
            }
        }


        /// <summary>
        /// Delete station.
        /// </summary>
        /// <param name="id">Station id</param>
        // DELETE: api/Stations/5
        [HttpDelete("{id}")]
        [Authorize(Permission.Stations.ADD)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteStation(Guid id)
        {
            if (id == default)
                return NotFound();

            using (var dbContext = _contextFactory.CreateDbContext())
            {
                var station = dbContext
                    .StationsForUser(UserId)
                    .Include(st => st.Tracks)
                    .FirstOrDefault(st => st.Id == id);

                if (station == null)
                    return NotFound();

                dbContext.Stations.Remove(station);
                var isSuccess = await dbContext.SaveChangesAsync().ConfigureAwait(false);
            }

            return Ok();
        }

        /// <summary>
        /// Get dates with existing track records for station
        /// </summary>
        [HttpGet("{stationId}/Dates")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        // GET: api/Stations/5/Dates
        public async Task<ActionResult<IEnumerable<DateTimeOffset>>> GetDates(Guid stationId)
        {
            if (stationId == default)
                return BadRequest();

            using var context = _contextFactory.CreateDbContext();
            var station = context.StationsForUser(UserId).FirstOrDefault();
            if (station == null)
                return NotFound();

            var datetimes = await context.Tracks
                .Where(t => t.StationId == stationId)
                .AsNoTracking()
                .Select(t => t.Created.Date.Date)
                .Distinct()
                .ToListAsync()
                .ConfigureAwait(false);

            return Ok(datetimes);
        }

        /// <summary>
        /// Get tracks for time interval
        /// </summary>
        /// <param name="stationId">Station id</param>
        /// <param name="from">Start date</param>
        /// <param name="to">End date</param>
        /// <returns></returns>
        [HttpGet("{stationId}/Tracks")]
        [Authorize(Permission.Stations.VIEW)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        // GET: api/Stations/5/Tracks?from=&to=
        public async Task<ActionResult<IEnumerable<TrackDto>>> GetTracks(Guid stationId, DateTimeOffset from, DateTimeOffset to)
        {
            if (stationId == default)
                return NotFound();

            using var context = _contextFactory.CreateDbContext();
            {
                var tracks = await context.Tracks
                    .AsNoTracking()
                    .Where(t => t.StationId == stationId)
                    .Where(t => t.Created < to)
                    .Where(t => t.Created > from)
                    .Select(t => new TrackDto(t.FullName, t.TrackName, t.Artist, t.Created.Date))
                    .ToListAsync()
                    .ConfigureAwait(false);

                return tracks;
            }
        }

        // TODO: rest api
        [HttpPatch("{stationId}")]
        [Authorize(Permission.Stations.RUN_STOP)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<bool>> Update(Guid stationId)
        {
            if (stationId == default)
                return NotFound();

            var isSuccess = Request.Form.TryGetValue("isRunning", out var isRunningStr);
            if (!isSuccess)
                return false;

            var actionIsSuccessfull = bool.TryParse(isRunningStr, out var isRunning) && isRunning
                ? await _ripperManager.RunAsync(stationId).ConfigureAwait(false)
                : await _ripperManager.StopAsync(stationId).ConfigureAwait(false);

            return new ActionResult<bool>(actionIsSuccessfull);
        }
    }
}
