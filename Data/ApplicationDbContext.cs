using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Reserva_Restaurantes.Models;

namespace Reserva_Restaurantes.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

public DbSet<Reserva_Restaurantes.Models.Restaurantes> Restaurantes { get; set; } = default!;

public DbSet<Reserva_Restaurantes.Models.Mesas> Mesas { get; set; } = default!;

public DbSet<Reserva_Restaurantes.Models.Reservas> Reservas { get; set; } = default!;

public DbSet<Reserva_Restaurantes.Models.Clientes> Clientes { get; set; } = default!;

public DbSet<Reserva_Restaurantes.Models.Pagamento> Pagamento { get; set; } = default!;
}