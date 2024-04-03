using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MrErsh.RadioRipper.IdentityDal;
using MrErsh.RadioRipper.Shared;
using MrErsh.RadioRipper.WebApi.Configuration;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MrErsh.RadioRipper.WebApi.Middleware
{
    [Obsolete]
    public sealed class JwtMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<JwtMiddleware> _logger;

        public JwtMiddleware(RequestDelegate next, ILogger<JwtMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context, UserManager<User> userManager, IOptionsSnapshot<SecurityJwtOptions> jwtOptions)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (token.NotNullOrWhiteSpace())
                await AttachUserToContext(context, userManager, jwtOptions, token);

            await _next(context);
        }

        private async Task AttachUserToContext(HttpContext context,
                                               UserManager<User> userManager,
                                               IOptionsSnapshot<SecurityJwtOptions> jwtOptions,
                                               string token)
        {
            try
            {
                var key = Encoding.ASCII.GetBytes(jwtOptions.Value.SigningKey);
                var validationParams = new TokenValidationParameters()
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = false
                };

                new JwtSecurityTokenHandler().ValidateToken(token, validationParams, out var validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var userId = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                var user = await userManager.FindByIdAsync(userId);
                if (user != null)
                    context.Items["User"] = user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Attach user to http context failed.");
            }
        }
    }
}
