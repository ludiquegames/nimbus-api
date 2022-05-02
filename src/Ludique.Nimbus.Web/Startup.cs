using Logitar.Email.SendGrid;
using Ludique.Nimbus.Infrastructure;
using Ludique.Nimbus.Infrastructure.Entities;
using Ludique.Nimbus.Web.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Ludique.Nimbus.Web
{
    public class Startup : StartupBase
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public override void ConfigureServices(IServiceCollection services)
        {
            base.ConfigureServices(services);
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddOpenApi();
            services.AddDbContext<NimbusDbContext>(options => options.UseNpgsql(_configuration.GetConnectionString(nameof(NimbusDbContext))));
            services.AddIdentity<User, Role>(options =>
            {
                options.SignIn.RequireConfirmedEmail = true;
            }).AddDefaultTokenProviders().AddEntityFrameworkStores<NimbusDbContext>();
            services.AddSendGrid();
            services.AddSingleton(_configuration.GetSection("Application").Get<ApplicationSettings>());
        }
        public override void Configure(IApplicationBuilder applicationBuilder)
        {
            if (applicationBuilder is WebApplication application)
            {
                if (application.Environment.IsDevelopment())
                {
                    application.UseSwagger();
                    application.UseSwaggerUI(config => config.SwaggerEndpoint(
                      "/swagger/v1/swagger.json",
                      "Nimbus API v1"
                    ));
                }

                application.UseHttpsRedirection();
                application.UseAuthorization();
                application.MapControllers();
            }

        }
    }
}
