using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace MrErsh.RadioRipper.IdentityDal
{
    internal sealed class DbInitializator
    {
        public void Run(ModelBuilder builder)
        {
            SetRoles(builder);
            SetClaims(builder);
            SetDefaultUsers(builder);
        }

        private static void SetRoles(ModelBuilder b)
        {
            Add(b,
                Role(SeedData.AdminRoleId, SeedData.AdminRoleName),
                Role(SeedData.UserRoleId, SeedData.UserRoleName));
        }

        private static void SetClaims(ModelBuilder b)
        {
            var rolClaimId = -1;
            void SetRolePermissionClaims(string roleId, params string[] permissions)
            {
                
                foreach (var p in permissions)
                {
                    Add(b, new IdentityRoleClaim<string>
                    {
                        Id = rolClaimId--,
                        RoleId = roleId,
                        ClaimType = Permission.PERMISSION_CLAIM_TYPE,
                        ClaimValue = p
                    });
                }
            }

            var userClaimId = -1;
            void SetUserPermissionClaims(string userId, params string[] permissions)
            {
                foreach (var p in permissions)
                {
                    Add(b, new IdentityUserClaim<string>
                    {
                        Id = userClaimId--,
                        UserId = userId,
                        ClaimType = Permission.PERMISSION_CLAIM_TYPE,
                        ClaimValue = p
                    });
                }
            }

            var adminRoleClaims = PermissionClaimsProvider.AllClaims.Select(c => c.Value).ToArray();
            SetRolePermissionClaims(SeedData.AdminRoleId, adminRoleClaims);
            SetRolePermissionClaims(SeedData.UserRoleId, Permission.Stations.ADD, Permission.Stations.RUN_STOP, Permission.Stations.VIEW);

            SetUserPermissionClaims(SeedData.DemoUserId, Permission.Stations.VIEW, Permission.Stations.RUN_STOP);
        }

        private static void SetDefaultUsers(ModelBuilder b)
        {
            var pswdHasher = new PasswordHasher<User>();
            User User(string id, string name, string password) =>
                new()
                {
                    Id = id,
                    UserName = name,
                    NormalizedUserName = name.ToUpper(),
                    PasswordHash = pswdHasher.HashPassword(null, password)
                };

            Add(b,
                User(SeedData.AdminUserId, SeedData.AdminUserName, SeedData.AdminUserPassword),
                User(SeedData.DemoUserId, SeedData.DemoUserName, SeedData.DemoUserPassword));

            Add(b, new IdentityUserRole<string> { RoleId = SeedData.AdminRoleId, UserId = SeedData.AdminUserId });
        }

        private static void Add<T>(ModelBuilder builder, params T[] entity) where T : class => builder.Entity<T>().HasData(entity);

        public static IdentityRole Role(string id, string name)
            => new() { Id = id, Name = name, NormalizedName = name.ToUpper() };
     
    }
}
