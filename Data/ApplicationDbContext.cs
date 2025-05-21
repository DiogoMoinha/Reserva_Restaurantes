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

public DbSet<Restaurantes> Restaurantes { get; set; } = default!;

public DbSet<Mesas> Mesas { get; set; } = default!;

public DbSet<Reservas> Reservas { get; set; } = default!;

public DbSet<Clientes> Clientes { get; set; } = default!;

public DbSet<Pagamento> Pagamento { get; set; } = default!;
}