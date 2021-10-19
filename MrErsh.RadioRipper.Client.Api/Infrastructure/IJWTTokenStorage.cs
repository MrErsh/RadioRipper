using JetBrains.Annotations;
using System.Threading;
using System.Threading.Tasks;

namespace MrErsh.RadioRipper.Client.Api.Infrastructure
{
    public interface IJWTTokenProvider
    {
        [ItemCanBeNull]
        Task<string> GetTokenAsync(CancellationToken cancellationToken = default);
    }

    public interface IJWTTokenStorage : IJWTTokenProvider
    {
        void Set([NotNull] string jwtToken);
        void Reset();
    }
}
