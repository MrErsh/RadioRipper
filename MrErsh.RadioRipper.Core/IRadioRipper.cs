using JetBrains.Annotations;
using System.Threading;
using System.Threading.Tasks;

namespace MrErsh.RadioRipper.Core
{
    public interface IRadioRipper
    {
        Task<MetadataHeader> ReadHeaderAsync(string url, [NotNull] RipperSettings settings,
                                             CancellationToken cancellationToken = default);
    }
}
