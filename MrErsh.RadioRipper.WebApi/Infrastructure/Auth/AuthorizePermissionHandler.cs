using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using MrErsh.RadioRipper.WebApi.Logging;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MrErsh.RadioRipper.WebApi.Auth
{
    public class AuthorizePermissionHandler : AuthorizationHandler<PermissionRequirement>
    {
        private readonly ILogger<AuthorizePermissionHandler> _logger;

        public AuthorizePermissionHandler(ILogger<AuthorizePermissionHandler> logger)
        {
            _logger = logger;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            if (!context.User.Identity.IsAuthenticated)
            {
                _logger.LogWarning(Events.Warnings.AuthenticationWarn, "API method unavailable - user is not Authenticated.");
                context.Fail();
            }
            else
            {
                if (context.User.HasClaim("Permission", requirement.Permission))
                {
                    context.Succeed(requirement);
                }
                else
                {
                    var currentUserId = ((ClaimsIdentity)context.User.Identity)
                        .Claims
                        .Single(cl => cl.Type == ClaimTypes.NameIdentifier)
                        .Value;

                    _logger.LogWarning("API method unavailable - insufficient privileges UserId={currentUserId}.", currentUserId);

                    context.Fail();
                }
            }

            return Task.CompletedTask;
        }
    }
}
