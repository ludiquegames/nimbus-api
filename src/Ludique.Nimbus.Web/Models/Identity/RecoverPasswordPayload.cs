using System.ComponentModel.DataAnnotations;

namespace Ludique.Nimbus.Web.Models.Identity
{
    public class RecoverPasswordPayload
    {
        [Required]
        public string Email { get; set; } = string.Empty;
    }
}
