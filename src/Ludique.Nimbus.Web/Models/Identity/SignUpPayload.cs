using System.ComponentModel.DataAnnotations;

namespace Ludique.Nimbus.Web.Models.Identity
{
    public class SignUpPayload
    {
        [Required]
        [EmailAddress]
        [StringLength(256)]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }
}
