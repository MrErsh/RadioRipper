using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace MrErsh.RadioRipper.IdentityDal
{
    public class DesignTimeIdentityDbContextFactory : IDesignTimeDbContextFactory<IdentityDbContext>
    {
        public IdentityDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<IdentityDbContext>();
            optionsBuilder.UseNpgsql("PORT=9070;TIMEOUT=15;POOLING=True;MINPOOLSIZE=1;MAXPOOLSIZE=20;COMMANDTIMEOUT=20;DATABASE=ripperidentity;HOST=host.docker.internal;USER ID=admin;PASSWORD=admin");
            return new IdentityDbContext(optionsBuilder.Options);
        }
    }
}
