using JetBrains.Annotations;
using MrErsh.RadioRipper.Core;
using MrErsh.RadioRipper.Model;
using Serilog;

namespace MrErsh.RadioRipper.WebApi.Bl
{
    public sealed class RipperFactory : IRipperFactory
    {
        private readonly ILogger _logger;

        public RipperFactory(ILogger logger)
        {
            _logger = logger;
        }

        public IRadioRipper Create() => new Ripper(_logger);

        public TimeredRadioRipper CreateTimered([NotNull] Station station)
        {
            var ripper = Create();
            return new TimeredRadioRipper(ripper, station, _logger);
        }
    }
}
