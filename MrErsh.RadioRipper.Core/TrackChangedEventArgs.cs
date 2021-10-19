using JetBrains.Annotations;
using System;

namespace MrErsh.RadioRipper.Core
{
    public sealed class TrackChangedEventArg : EventArgs
    {
        public TrackChangedEventArg(MetadataHeader info) => Info = info;

        [NotNull]
        public MetadataHeader Info { get; }
    }
}
