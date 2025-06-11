namespace Reserva_Restaurantes.Data.DBInitializerDev;

public static class DBInitializerExtension
{
    public static IApplicationBuilder UseItToSeedSqlServer(this IApplicationBuilder app) {
        ArgumentNullException.ThrowIfNull(app, nameof(app));

        using var scope = app.ApplicationServices.CreateScope();
        var services = scope.ServiceProvider;
        try {
            var context = services.GetRequiredService<ApplicationDbContext>();
            DBInitializer.Initialize(context);
        }
        catch (Exception ex) {

        }

        return app;
    }
}
