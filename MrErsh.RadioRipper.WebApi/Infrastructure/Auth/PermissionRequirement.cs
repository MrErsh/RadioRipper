using Microsoft.AspNetCore.Authorization;

namespace MrErsh.RadioRipper.WebApi.Auth
{
    public record PermissionRequirement(string Permission) : IAuthorizationRequirement { }
}
