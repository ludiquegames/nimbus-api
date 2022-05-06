using Logitar.Email;
using Ludique.Nimbus.Infrastructure.Entities;
using Ludique.Nimbus.Web.Models.Identity;
using Ludique.Nimbus.Web.Services;
using Ludique.Nimbus.Web.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Ludique.Nimbus.Web.Controllers
{
    [ApiController]
    [Route("identity")]
    public class IdentityController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly IMessageService _messageService;
        private readonly ApplicationSettings _applicationSettings;
        private readonly ITokenService _tokenService;

        public IdentityController(UserManager<User> userManager, IMessageService messageService, ApplicationSettings applicationSettings, ITokenService tokenService)
        {
            _userManager = userManager;
            _messageService = messageService;
            _applicationSettings = applicationSettings;
            _tokenService = tokenService;
        }

        [HttpPost("sign/in")]
        public async Task<ActionResult<TokenModel>> SignIn([FromBody] SignInPayload payload)
        {
            User? user = await _userManager.FindByEmailAsync(payload.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, payload.Password))
            {
                return BadRequest(new { code = "IncorrectCredentials" });
            }
            TokenModel token = await _tokenService.GenerateAsync(user);
            return Ok(token);
        }

        [HttpPost("sign/up")]
        public async Task<ActionResult> SignUp([FromBody] SignUpPayload payload, CancellationToken cancellationToken)
        {
            var user = new User
            {
                Email = payload.Email,
                UserName = payload.Email
            };

            IdentityResult result = await _userManager.CreateAsync(user, payload.Password);

            if (!result.Succeeded)
            {
                return BadRequest(result);
            }

            string token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            var message = new Message(
                Resources.Email.AccountCreation_Subject,
                Resources.Email.AccountCreation_Body
                    .Replace("{id}", user.Id.ToString())
                    .Replace("{token}", token)
                    .Replace("{baseUrl}", _applicationSettings.BaseUrl),
                new[] { new Recipient(payload.Email) }
            );

            await _messageService.SendAsync(message, cancellationToken);
            
            return NoContent();
        }

        [HttpPost("confirm")]
        public async Task<ActionResult> ConfirmAsync([FromBody] ConfirmPayload payload, CancellationToken cancellationToken)
        {
            User? user = await _userManager.FindByIdAsync(payload.Id.ToString());
            
            if (user == null)
            {
                return BadRequest(new { Code = "InvalidId" });
            }

            IdentityResult result = await _userManager.ConfirmEmailAsync(user, payload.Token);

            if (!result.Succeeded)
            {
                return BadRequest(result);
            }

            var message = new Message(
                Resources.Email.AccountConfirmed_Subject,
                Resources.Email.AccountConfirmed_Body,
                new[] { new Recipient(user.Email) }
            );

            await _messageService.SendAsync(message, cancellationToken);

            return NoContent();
        }

        [HttpPost("password/change")]
        [Authorize]
        public async Task<ActionResult> ChangePasswordAsync([FromBody] ChangePasswordPayload payload)
        {
            User user = await _userManager.GetUserAsync(User);
            IdentityResult result = await _userManager.ChangePasswordAsync(user, payload.CurrentPassword, payload.NewPassword);
            if (!result.Succeeded)
            {
                return BadRequest(result);
            }
            return NoContent();
        }

        [HttpPost("password/recover")]
        public async Task<ActionResult> RecoverPassword([FromBody] RecoverPasswordPayload payload, CancellationToken cancellationToken)
        {
            User? user = await _userManager.FindByEmailAsync(payload.Email);
            if (user == null)
            {
                return BadRequest(new { code = "IncorrectCredentials" });
            }

            string token = await _userManager.GeneratePasswordResetTokenAsync(user);

            var message = new Message(
                Resources.Email.PasswordRecovery_Subject,
                Resources.Email.PasswordRecovery_Body
                    .Replace("{id}", user.Id.ToString())
                    .Replace("{token}", token)
                    .Replace("{baseUrl}", _applicationSettings.BaseUrl),
                new[] { new Recipient(payload.Email) }
            );

            await _messageService.SendAsync(message, cancellationToken);

            return NoContent();
        }

        [HttpPost("password/reset")]
        public async Task<ActionResult> ResetPassword([FromBody] ResetPasswordPayload payload, CancellationToken cancellationToken)
        {
            User? user = await _userManager.FindByIdAsync(payload.Id.ToString());

            if (user == null)
            {
                return BadRequest(new { Code = "InvalidId" });
            }

            IdentityResult result = await _userManager.ResetPasswordAsync(user, payload.Token, payload.NewPassword);

            if (!result.Succeeded)
            {
                return BadRequest(result);
            }

            var message = new Message(
                Resources.Email.AccountConfirmed_Subject,
                Resources.Email.AccountConfirmed_Body,
                new[] { new Recipient(user.Email) }
            );

            await _messageService.SendAsync(message, cancellationToken);

            return NoContent();
        }
    }
}
