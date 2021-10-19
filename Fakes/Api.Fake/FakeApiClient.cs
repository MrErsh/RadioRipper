#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
using MrErsh.RadioRipper.Model;
using MrErsh.RadioRipper.Model.Dto;
using Refit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading.Tasks;

namespace MrErsh.RadioRipper.Client.Api.Fake
{
    public class FakeApiClient : IApiClient
    {
        public async Task<ApiResponse<Station>> AddStationAsync([Body] AddStationDto station)
        {
            var t = new ApiResponse<Station>(new System.Net.Http.HttpResponseMessage(),
                                             new Station { Url = station.Url, Name = station.Name },
                                             null);
            await Task.Delay(2000);
            return t;
        }

        public async Task<bool> ChangeIsRunningAsync(Guid id, [Body(BodySerializationMethod.UrlEncoded)] ChangeIsRunningParams param)
        {
            await Task.Delay(2000);
            return true;
        }

        public Task<ApiResponse<object>> DeleteStationAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<ApiResponse<IReadOnlyCollection<Station>>> GetStationsAsync()
        {
            var data = await GetData<List<Station>>();
            var response = new ApiResponse<IReadOnlyCollection<Station>>(
                new System.Net.Http.HttpResponseMessage(),
                data,
                null);
            return response;
        }

        public async Task<IReadOnlyCollection<StationDto>> GetStationsShortAsync()
        {
            var result = await GetData<List<StationDto>>();
            var t = 0;
            foreach (var st in result)
            {
                st.Dates.Add(DateTime.Today.AddDays(--t));
            }
            return result;
        }

        public async Task<IReadOnlyCollection<TrackDto>> GetTracksAsync(Guid stationId, DateTimeOffset from, DateTimeOffset to)
        {
            return await GetData<List<TrackDto>>();
        }

        public Task<List<DateTime>> GetDatesAsync(Guid stationId)
        {
            var t = DateTime.Today;
            var list = new List<DateTime> { t, t.AddDays(-1), t.AddDays(-10), t.AddMonths(-1).AddDays(2) };
            return Task.FromResult(list);
        }

        private async static Task<T> GetData<T>([CallerMemberName] string fileName = null)
        {
            await Task.Delay(2000);
            //Thread.Sleep(2000);
            var asm = Assembly.GetExecutingAssembly();
            var name = $"{asm.GetName().Name}.Data.{fileName}.json";
            using var stream = asm.GetManifestResourceStream(name);
            using var reader = new StreamReader(stream);
            var t = reader.ReadToEnd();
            return JsonSerializer.Deserialize<T>(t);
        }
    }
}
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
