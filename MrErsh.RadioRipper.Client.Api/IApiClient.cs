using MrErsh.RadioRipper.Model;
using MrErsh.RadioRipper.Model.Dto;
using Refit;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MrErsh.RadioRipper.Client.Api
{
    // TOOD: не, не так
    public sealed class ChangeIsRunningParams {[AliasAs("isRunning")] public bool IsRunning { get; set; } }

    // По хорошему это должно генерироваться
    public interface IApiClient
    {
        [Get("/Stations")]
        Task<ApiResponse<IReadOnlyCollection<Station>>> GetStationsAsync();

        [Get("/Stations/Short")]
        Task<IReadOnlyCollection<StationDto>> GetStationsShortAsync();

        [Post("/Stations")]
        Task<ApiResponse<Station>> AddStationAsync([Body] AddStationDto station);

        [Delete("/Stations/{id}")]
        Task<ApiResponse<object>> DeleteStationAsync(Guid id);

        /// <summary>
        /// Список дат, для которых есть записи. Время локальное
        /// </summary>
        /// <param name="stationId">Id станции</param>
        /// <returns></returns>
        [Get("/Stations/{stationId}/Dates")]
        Task<List<DateTime>> GetDatesAsync(Guid stationId);

        [Get("/Stations/{stationId}/Tracks?from={from}&to={to}")]
        Task<IReadOnlyCollection<TrackDto>> GetTracksAsync(Guid stationId, DateTimeOffset from, DateTimeOffset to);

        [Patch("/Stations/{id}")]
        Task<bool> ChangeIsRunningAsync(Guid id, [Body(BodySerializationMethod.UrlEncoded)] ChangeIsRunningParams param);
    }
}
