using JetBrains.Annotations;
using System;

namespace MrErsh.RadioRipper.Model
{
    public sealed class Track : Entity
    {
        public string TrackName { get; set; }

        public string Artist { get; set; }

        public string FullName { get; set; }

        // TODO: в перспективе удалить
        public string MetadataHeader { get; set; }

        /// Utc
        public DateTimeOffset Created { get; set; }

        [CanBeNull]
        public Guid? StationId { get; set; }

        [CanBeNull]
        public Station Station { get; set; }
    }
}
