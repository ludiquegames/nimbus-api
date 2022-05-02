using Microsoft.OpenApi.Models;

namespace Ludique.Nimbus.Web
{
    public static class OpenApiExtensions
    {
        public static void AddOpenApi(this IServiceCollection services)
        {
            ArgumentNullException.ThrowIfNull(services);

            services.AddSwaggerGen(config =>
            {
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
