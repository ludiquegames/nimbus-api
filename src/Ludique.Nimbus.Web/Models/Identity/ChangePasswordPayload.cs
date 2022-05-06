using System.ComponentModel.DataAnnotations;

namespace Ludique.Nimbus.Web.Models.Identity
{
    public class ChangePasswordPayload
    {
        [Required]
        public string CurrentPassword { get; set; } = string.Empty;
        [Required]
        public string NewPassword { get; set; } = string.Empty;
    }
}
