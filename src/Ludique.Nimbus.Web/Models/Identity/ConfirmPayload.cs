using System.ComponentModel.DataAnnotations;

namespace Ludique.Nimbus.Web.Models.Identity
{
    public class ConfirmPayload
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public string Token { get; set; } = string.Empty;
    }
}
