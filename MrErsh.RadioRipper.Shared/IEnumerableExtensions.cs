using JetBrains.Annotations;
using System;
using System.Collections.Generic;

namespace MrErsh.RadioRipper.Shared
{
    public static class IEnumerableExtensions
    {
        public static void ForAll<T>([CanBeNull] this IEnumerable<T> source, [NotNull] Action<T> action)
        {
            if (source == null)
                return;

            foreach (var item in source)
                action(item);
        }
    }
}
