using JetBrains.Annotations;
using MrErsh.RadioRipper.Model;
using System.Linq;

namespace MrErsh.RadioRipper.Dal
{
    public static class RadioDbExtensions
    {
        public static IQueryable<Station> ForUser(this IQueryable<Station> source, [NotNull] string userId)
            => source.Where(st => st.UserId == userId);

        public static IQueryable<Station> StationsForUser(this RadioDbContext context, [NotNull] string userId)
            => context.Stations.Where(st => st.UserId == userId);
    }
}
