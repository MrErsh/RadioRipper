using JetBrains.Annotations;
using System.Collections.Generic;

namespace MrErsh.RadioRipper.Shared
{
    public static class StringExtensions
    {
        public static bool IsNullOrWhiteSpace(this string value) => string.IsNullOrWhiteSpace(value);
        public static bool NotNullOrWhiteSpace(this string value) => !string.IsNullOrWhiteSpace(value);

        [NotNull]
        public static string JoinToString(this IEnumerable<string> strings, string separator)
        {
            return strings == null
                ? string.Empty
                : string.Join(separator, strings);
        }
    }
}
