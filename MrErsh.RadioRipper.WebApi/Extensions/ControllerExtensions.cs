using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;

namespace MrErsh.RadioRipper.WebApi.ApiControllers
{
    public static class ControllerExtensions
    {
        [NotNull]
        public static string GetCurrentUserId(this ControllerBase controller)
        {
            return ((ClaimsIdentity)controller.HttpContext.User.Identity)
                .Claims
                .Single(c => c.Type == ClaimTypes.NameIdentifier)
                .Value;
        }
    }
}
