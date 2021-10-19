using JetBrains.Annotations;
using System.Threading;

namespace MrErsh.RadioRipper.Core
{
    public interface IRadioRipper
    {
        [NotNull]
        MetadataHeader ReadHeader(string url, [NotNull] RipperSettings settings, CancellationToken cancellationToken = default);
    }
}
