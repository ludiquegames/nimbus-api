using System.ComponentModel.DataAnnotations;

namespace Ludique.Nimbus.Web.Models.Identity
{
    public class ResetPasswordPayload
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public string Token { get; set; } = string.Empty;

        [Required]
        public string NewPassword { get; set; } = string.Empty;
    }
}
