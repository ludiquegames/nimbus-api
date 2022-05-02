using Ludique.Nimbus.Infrastructure.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Ludique.Nimbus.Infrastructure
{
    public class NimbusDbContext : IdentityDbContext<User, Role, Guid>
    {
        public NimbusDbContext(DbContextOptions<NimbusDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}
