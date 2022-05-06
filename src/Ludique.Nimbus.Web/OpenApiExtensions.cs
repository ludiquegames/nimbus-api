using Ludique.Nimbus.Web.Settings;
using Microsoft.OpenApi.Models;

namespace Ludique.Nimbus.Web
{
    public static class OpenApiExtensions
    {
        private const string AuthorizationHeader = "Authorization";
        private const string OAuth2Scheme = "oauth2";

        public static void AddOpenApi(this IServiceCollection services, JwtSettings jwtSettings)
        {
            ArgumentNullException.ThrowIfNull(services);

            services.AddSwaggerGen(config =>
            {
                config.AddSecurityDefinition(jwtSettings.Type, new OpenApiSecurityScheme
                {
                    Description = "Enter your token in the input below.",
                    In = ParameterLocation.Header,
                    Name = AuthorizationHeader,
                    Scheme = jwtSettings.Type,
                    Type = SecuritySchemeType.Http
                });

                config.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                  {
                    new OpenApiSecurityScheme
                    {
                      In = ParameterLocation.Header,
                      Name = jwtSettings.Type,
                      Reference = new OpenApiReference
                      {
                        Id = jwtSettings.Type,
                        Type = ReferenceType.SecurityScheme
                      },
                      Scheme = OAuth2Scheme
                    },
                    new List<string>()
                  }
                });

                string name = "v1";
                config.SwaggerDoc(name, new OpenApiInfo
                {
                    Contact = new OpenApiContact
                    {
                        Email = "ludiquegames@gmail.com",
                        Name = "Raphaël Pion",
                        Url = new Uri("https://raphpion.io")
                    },
                    Description = "Nimbus Account Management API",
                    License = new OpenApiLicense
                    {
                        Name = "Use under MIT",
                        Url = new Uri("https://github.com/ludiquegames/nimbus-api/blob/main/LICENSE")
                    },
                    Title = "Nimbus API",
                    Version = "v1"
                });
            });
        }
    }
}
