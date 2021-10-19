using JetBrains.Annotations;
using MrErsh.RadioRipper.Core;
using MrErsh.RadioRipper.Model;

namespace MrErsh.RadioRipper.WebApi.Bl
{
    public interface IRipperFactory
    {
        IRadioRipper Create();
        TimeredRadioRipper CreateTimered([NotNull] Station station);
    }
}
