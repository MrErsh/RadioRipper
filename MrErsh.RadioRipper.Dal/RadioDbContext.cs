using Microsoft.EntityFrameworkCore;
using MrErsh.RadioRipper.Model;

namespace MrErsh.RadioRipper.Dal
{
    public sealed class RadioDbContext : DbContext
    {
        public RadioDbContext(DbContextOptions<RadioDbContext> options) : base(options) {}

        public DbSet<Track> Tracks { get; set; }

        public DbSet<Station> Stations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Station>().HasIndex(st => st.UserId);
        }
    }
}
