using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

class Program
{
    static void Main()
    {
        // Appel de la méthode CreateHostBuilder pour configurer et lancer l'application
        CreateHostBuilder().Build().Run();
    }

    // Méthode pour configurer l'hôte de l'application
    public static IHostBuilder CreateHostBuilder() =>
        Host.CreateDefaultBuilder()
            .ConfigureWebHostDefaults(webBuilder =>
            {
                // Utilisation de la classe Startup pour configurer l'application
                webBuilder.UseStartup<Startup>();
            });

}

// Classe Startup pour configurer l'application ASP.NET Core
public class Startup
{
    // Méthode pour ajouter des services à l'injection de dépendances
    public void ConfigureServices(IServiceCollection services)
    {
        // Ajout des services
    }

    // Méthode pour configurer l'application et ses composants
    public void Configure(IApplicationBuilder app)
    {
        // Configurer le routage des requêtes HTTP
        app.UseRouting();

        // Configurer les points de terminaison pour les requêtes HTTP
        app.UseEndpoints(endpoints =>
        {
            // Définir un point de terminaison pour la route principale "/"
            endpoints.MapGet("/", async context =>
            {
                // Répondre avec un message simple
                await context.Response.WriteAsync("Bienvenue sur le server");
            });
        });
    }
}
