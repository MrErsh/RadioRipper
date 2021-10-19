using System;
using System.Threading.Tasks;

namespace MrErsh.RadioRipper.WebApi.Bl
{
    public interface IRipperManager
    {
        Task RestartAllAsync();
        Task<bool> RunAsync(Guid idStation);
        Task<bool> StopAsync(Guid idStation);
    }
}
