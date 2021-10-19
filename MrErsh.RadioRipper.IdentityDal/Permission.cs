using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace MrErsh.RadioRipper.IdentityDal
{
    public static class Permission
    {      
        public const string PERMISSION_CLAIM_TYPE = "Permission";

        public static class Stations
        {
            public const string VIEW = "Stations.View";
            public const string RUN_STOP = "Stations.Run"; // если run то и стоп
            public const string ADD = "Stations.Add";
            public const string STOP_FOR_OTHER_USERS = "Stations.StopForOtherUsers";
        }

        public static class Users 
        {
            public const string ADD = "Users.Add";
            public const string DELETE = "Users.Remove";
            public const string EDIT = "Users.Edit";
        }
    }

    public static class PermissionClaimsProvider
    {
        public static readonly IReadOnlyCollection<Claim> AllClaims;

        static PermissionClaimsProvider()
        {
            AllClaims = new[]
            {
                Permission.Stations.VIEW,
                Permission.Stations.RUN_STOP,
                Permission.Stations.ADD,
                Permission.Stations.STOP_FOR_OTHER_USERS,

                Permission.Users.ADD,
                Permission.Users.DELETE,
                Permission.Users.EDIT
            }
            .Select(p => new Claim(Permission.PERMISSION_CLAIM_TYPE, p))
            .ToList();           
        }
    }
}
