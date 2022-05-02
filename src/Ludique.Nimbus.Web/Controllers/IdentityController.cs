using Logitar.Email;
using Ludique.Nimbus.Infrastructure.Entities;
using Ludique.Nimbus.Web.Models.Identity;
using Ludique.Nimbus.Web.Settings;
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

        public IdentityController(UserManager<User> userManager, IMessageService messageService, ApplicationSettings applicationSettings)
        {
            _userManager = userManager;
            _messageService = messageService;
            _applicationSettings = applicationSettings;
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
                new[] { new Recipient(payload.Email) } // déclare un Recipient[]
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
    }
}
