using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace MrErsh.RadioRipper.Dal.Services
{
    public class DesignTimeRadioDbFactory : IDesignTimeDbContextFactory<RadioDbContext>
    {
        public RadioDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<RadioDbContext>();
            optionsBuilder.UseNpgsql("PORT=9070;TIMEOUT=15;POOLING=True;MINPOOLSIZE=1;MAXPOOLSIZE=20;COMMANDTIMEOUT=20;DATABASE=ripper;HOST=host.docker.internal;USER ID=admin;PASSWORD=admin");
            return new RadioDbContext(optionsBuilder.Options);
        }
    }
}
