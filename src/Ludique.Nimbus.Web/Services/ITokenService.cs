using Ludique.Nimbus.Infrastructure.Entities;
using Ludique.Nimbus.Web.Models.Identity;

namespace Ludique.Nimbus.Web.Services
{
    public interface ITokenService
    {
        Task<TokenModel> GenerateAsync(User user);
    }
}
