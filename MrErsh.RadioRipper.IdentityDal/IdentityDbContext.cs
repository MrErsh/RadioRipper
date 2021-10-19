using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MrErsh.RadioRipper.IdentityDal
{
    public class IdentityDbContext : IdentityDbContext<User>
    {
        public IdentityDbContext(DbContextOptions<IdentityDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<User>()
                .Ignore(u=> u.PhoneNumber)
                .Ignore(u=> u.PhoneNumberConfirmed)
                .Ignore(u=> u.EmailConfirmed)
                .Ignore(u=> u.TwoFactorEnabled)
                .Ignore(u=> u.LockoutEnd)
                .Ignore(u=> u.LockoutEnabled)
                .Ignore(u=> u.AccessFailedCount)
                .Ignore(u => u.Email)
                .Ignore(u => u.NormalizedEmail);

            builder.Entity<User>().ToTable("User");
            builder.Entity<IdentityRole>().ToTable("Role");
            builder.Entity<IdentityUserRole<string>>().ToTable("UserRole");
            builder.Entity<IdentityRoleClaim<string>>().ToTable("RoleClaim");

            builder.Entity<IdentityUserClaim<string>>().ToTable("UserClaim");
            builder.Entity<IdentityUserLogin<string>>().ToTable("UserLogin");
            builder.Entity<IdentityUserToken<string>>().ToTable("UserToken");

            new DbInitializator().Run(builder);
        }
    }
}
