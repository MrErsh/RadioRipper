using JetBrains.Annotations;
using System;

namespace MrErsh.RadioRipper.Core
{
    public sealed class TrackChangedEventArg : EventArgs
    {
        public TrackChangedEventArg(Guid stationId, [NotNull] MetadataHeader info)
        {
            StationId = stationId;
            Info = info;
        }

        public Guid StationId { get; }

        [NotNull]
        public MetadataHeader Info { get; }
    }
}
