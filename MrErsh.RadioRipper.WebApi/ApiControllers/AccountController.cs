using JetBrains.Annotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MrErsh.RadioRipper.IdentityDal;
using MrErsh.RadioRipper.Model.Dto;
using MrErsh.RadioRipper.Shared;
using MrErsh.RadioRipper.WebApi.Auth;
using MrErsh.RadioRipper.WebApi.Configuration;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MrErsh.RadioRipper.WebApi.ApiControllers
{
    [Route("api/[controller]")]
    [ApiController, RequireHttps]
    public class AccountController : Controller
    {
        #region Fields

        private readonly UserManager<User> _userManager;
        private readonly IJwtEncodingDescription _jwtEncodeDescriptor;
        private readonly SecurityJwtOptions _jwtOptions;
        private readonly ILogger<AccountController> _logger;
        private readonly IUserClaimsPrincipalFactory<User> _claimsFactory;

        #endregion

        #region Constructor
        public AccountController(UserManager<User> userManager,
                                 IJwtEncodingDescription jwtEncodeDescriptor,
                                 IOptions<SecurityJwtOptions> jwtOptions,
                                 ILogger<AccountController> logger,
                                 IUserClaimsPrincipalFactory<User> claimsFactory)
        {
            _userManager = userManager;
            _jwtEncodeDescriptor = jwtEncodeDescriptor;
            _jwtOptions = jwtOptions.Value;
            _logger = logger;
            _claimsFactory = claimsFactory;
        }
        #endregion

        #region Actions

        // POST: api/Account
        [HttpPost, AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<string>> Login([FromBody] LoginDto loginDto)
        {
            User user = null;
            try
            {
                user = await _userManager.FindByNameAsync(loginDto.UserName).ConfigureAwait(false);
            }
            catch { }
            user ??= await _userManager.FindByNameAsync(loginDto.UserName).ConfigureAwait(false);

            if (user == null && loginDto.CreateNew)
            {
                var (createdUsr, errors) = await CreateUserAsync(loginDto).ConfigureAwait(false);
                if (createdUsr == null)
                    return Unauthorized(errors.Select(err => err.Description).JoinToString(Environment.NewLine));

                user = createdUsr;
            }

            if (user == null)
            {
                _logger.LogWarning("User '{login}' does't exists", loginDto.UserName);
                return Unauthorized("User not found");
            }

            var isPasswordCorrect = await _userManager.CheckPasswordAsync(user, loginDto.Password);
            if (!isPasswordCorrect)
            {
                _logger.LogWarning("Wrong password for user '{user}'", loginDto.UserName);
                return Unauthorized("Password is invalid");
            }

            var claims = await _claimsFactory.CreateAsync(user);
            var token = CreateJwtToken(claims.Claims.ToList());
            var result = new JwtSecurityTokenHandler().WriteToken(token);
            return Ok(result);
        }

        // DELETE: api/Account
        [HttpDelete("{id}")]
        [Authorize(Permission.Users.DELETE)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DeleteAccount(Guid id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString()).ConfigureAwait(false);
            if (user == null)
            {
                _logger.LogWarning("User '{UserId}' does't exists", id);
                return Ok();
            }

            var result = await _userManager.DeleteAsync(user).ConfigureAwait(false);
            if (result.Succeeded)
                return Ok();

            LogErrors(result.Errors);

            return new ContentResult()
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                Content = result.Errors.Select(err => err.Description).JoinToString(Environment.NewLine)
            };
        }

        #endregion

        #region Private methods

        private JwtSecurityToken CreateJwtToken(ICollection<Claim> claims)
        {
            var expDate = DateTime.Now.AddMinutes(_jwtOptions.SessionLifetimeMin);

            return new JwtSecurityToken(
                _jwtOptions.Issuer,
                _jwtOptions.Audience,
                claims,
                expires: expDate,
                signingCredentials: new SigningCredentials(_jwtEncodeDescriptor.Key, _jwtEncodeDescriptor.SigningAlgorithm));
        }

        // TODO: use transaction
        [ItemCanBeNull]
        private async Task<(User, IEnumerable<IdentityError>)> CreateUserAsync(LoginDto loginDto)
        {
            var newUser = new User { UserName = loginDto.UserName };

            var result = await _userManager.CreateAsync(newUser, loginDto.Password).ConfigureAwait(false);
            if (result.Succeeded)
            {
                var currentUser = await _userManager.FindByNameAsync(loginDto.UserName).ConfigureAwait(false);
                var rolResult = await _userManager.AddToRoleAsync(currentUser, SeedData.UserRoleName);
                if (!rolResult.Succeeded)
                {
                    LogErrors(rolResult.Errors);
                    return (null, rolResult.Errors);
                }

                return (currentUser, null);
            }
            else
            {
                LogErrors(result.Errors);
                return (null, result.Errors);
            }
        }

        private void LogErrors(IEnumerable<IdentityError> errors)
        {
            foreach (var error in errors)
                _logger.LogError("{code}: {description}", error.Code, error.Description);
        }

        #endregion
    }
}
