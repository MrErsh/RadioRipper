using JetBrains.Annotations;
using System.Text.RegularExpressions;

namespace MrErsh.RadioRipper.Core
{
    public sealed class MetadataHeader
    {
        private static readonly Regex _streamTitleRegex = new Regex("StreamTitle='(?<title_group>[^'].*)';", RegexOptions.Compiled);
        private static readonly Regex _artistTrackRegex = new Regex("(?<artist>.*) - (?<name>.*)");

        public MetadataHeader([CanBeNull] string origin)
        {
            if (origin == null)
                return;

            Origin = origin.Trim('\0');
            var matches = _streamTitleRegex.Match(origin);
            var title = matches.Groups["title_group"].Value.Trim().Trim('\0');
            StreamTitle = title
                .Replace(@"';StreamUrl='", string.Empty); // TODO: в regex

            var taMatches = _artistTrackRegex.Match(StreamTitle);
            Artist = taMatches.Groups["artist"].Value.Trim();
            TrackName = taMatches.Groups["name"].Value.Trim();
        }

        [CanBeNull]
        public string Origin { get; }

        [NotNull]
        public string StreamTitle { get; } = string.Empty;

        [NotNull]
        public string TrackName { get; } = string.Empty;

        [NotNull]
        public string Artist { get; } = string.Empty;
    }
}
