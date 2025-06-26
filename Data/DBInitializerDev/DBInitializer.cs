using Reserva_Restaurantes.Models;

namespace Reserva_Restaurantes.Data.DBInitializerDev;

public class DBInitializer
{
    
    internal static async void Initialize(ApplicationDbContext dbContext) {

     /*
      * https://stackoverflow.com/questions/70581816/how-to-seed-data-in-net-core-6-with-entity-framework
      * 
      * https://learn.microsoft.com/en-us/aspnet/core/data/ef-mvc/intro?view=aspnetcore-6.0#initialize-db-with-test-data
      * https://github.com/dotnet/AspNetCore.Docs/blob/main/aspnetcore/data/ef-mvc/intro/samples/5cu/Program.cs
      * https://learn.microsoft.com/en-us/dotnet/fundamentals/code-analysis/style-rules/ide0300
      */


     ArgumentNullException.ThrowIfNull(dbContext, nameof(dbContext));
     dbContext.Database.EnsureCreated();

     // var auxiliar
     bool haAdicao = false;



     // Se não houver Restaurantes, cria-as
     var restaurantes = Array.Empty<Restaurantes>();

     if (dbContext.Restaurantes.Count() == 0) {
        restaurantes = [
           new Restaurantes { Id = 1, Nome = "Batatinha", Endereco = "Ruas das quintas, Trambulhão", CodPostal = "3456-789", HoraAbertura = DateTime.Today.AddHours(11).AddMinutes(30), HoraFecho = DateTime.Today.AddHours(22).AddMinutes(0) },
           new Restaurantes { Id = 2, Nome = "Parvinhos", Endereco = "Ruas das Paroquias, Casal do Arrial", CodPostal = "1234-987", HoraAbertura = DateTime.Today.AddHours(12).AddMinutes(0), HoraFecho = DateTime.Today.AddHours(21).AddMinutes(45) },
           new Restaurantes { Id = 3, Nome = "Canto do Gordo", Endereco = "Bairo do Padre, Beirais", CodPostal = "9876-321", HoraAbertura = DateTime.Today.AddHours(11).AddMinutes(0), HoraFecho = DateTime.Today.AddHours(22).AddMinutes(30) }
           //adicionar outras categorias
        ];
        await dbContext.Restaurantes.AddRangeAsync(restaurantes);
        haAdicao = true;
     }


     // Se não houver Clientes, cria-os
     var clientes = Array.Empty<Clientes>();
     if (!dbContext.Clientes.Any()) {
        clientes = [
           new Clientes { Nome="Diana Henriques", Email = "dianaHen@gmail.com", Telefone= "919876543"},
           new Clientes { Nome="Pedro Paiva", Email = "PP26@gmail.com", Telefone= "923522226"},
           new Clientes { Nome="Horácio Alexandre", Email = "HorAlex@gmail.com", Telefone= "967854321"}
          ];
        await dbContext.Clientes.AddRangeAsync(clientes);
        haAdicao = true;
     }



     // Se não houver Mesas, cria-as
     var mesas = Array.Empty<Mesas>();
     if (!dbContext.Mesas.Any()) {
        mesas = [
           new Mesas { NumMesa = 1, Capacidade = 4, RestauranteFK = 1},
           new Mesas { NumMesa = 3, Capacidade = 6, RestauranteFK = 2},
           new Mesas { NumMesa = 5, Capacidade = 12, RestauranteFK = 3}
           ];
        await dbContext.Mesas.AddRangeAsync(mesas);
        haAdicao = true;
     }


     try {
        if (haAdicao) {
           // tornar persistentes os dados
           dbContext.SaveChanges();
        }
     }
     catch (Exception ex) {

        throw;
     }
  }
}