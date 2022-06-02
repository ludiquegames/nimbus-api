using Logitar.Email.SendGrid;
using Ludique.Nimbus.Infrastructure;
using Ludique.Nimbus.Infrastructure.Entities;
using Ludique.Nimbus.Web.Services;
using Ludique.Nimbus.Web.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Ludique.Nimbus.Web
{
    public class Startup : StartupBase
    {
        private readonly IConfiguration _configuration;
        private readonly JwtSettings _jwtSettings;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
            _jwtSettings = _configuration.GetSection("Jwt").Get<JwtSettings>();
        }

        public override void ConfigureServices(IServiceCollection services)
        {
            base.ConfigureServices(services);
            services.AddCors();
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddOpenApi(_jwtSettings);
            services.AddDbContext<NimbusDbContext>(options => options.UseNpgsql(_configuration.GetConnectionString(nameof(NimbusDbContext))));
            services.AddIdentity<User, Role>(options =>
            {
                options.SignIn.RequireConfirmedEmail = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireDigit = false;
            }).AddDefaultTokenProviders().AddEntityFrameworkStores<NimbusDbContext>();
            services.AddSendGrid();
            services.AddSingleton(_configuration.GetSection("Application").Get<ApplicationSettings>());
            services.AddSingleton(_jwtSettings);
            services.AddScoped<ITokenService, JwtService>();
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new()
                {
                    ValidAudience = _jwtSettings.Audience,
                    ValidIssuer = _jwtSettings.Issuer,
                    ValidateAudience = true,
                    ValidateIssuer = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_jwtSettings.Secret))
                };
            });
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

                application.UseCors(builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
                application.UseHttpsRedirection();
                application.UseAuthentication();
                application.UseAuthorization();
                application.MapControllers();
            }

        }
    }
}
