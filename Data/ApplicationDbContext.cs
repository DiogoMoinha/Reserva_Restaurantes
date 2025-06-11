using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Reserva_Restaurantes.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Reserva_Restaurantes.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // 'importa' todo o comportamento do m�todo, 
        // aquando da sua defini��o na SuperClasse
        base.OnModelCreating(modelBuilder);

        // criar os perfis de utilizador da nossa app
        modelBuilder.Entity<IdentityRole>().HasData(
            new IdentityRole { Id = "a", Name = "Administrador", NormalizedName = "ADMINISTRADOR" });

        // criar um utilizador para funcionar como ADMIN
        // fun��o para codificar a password
        var hasher = new PasswordHasher<IdentityUser>();
        // cria��o do utilizador
        modelBuilder.Entity<IdentityUser>().HasData(
            new IdentityUser
            {
                Id = "admin",
                UserName = "admin@mail.pt",
                NormalizedUserName = "ADMIN@MAIL.PT",
                Email = "admin@mail.pt",
                NormalizedEmail = "ADMIN@MAIL.PT",
                EmailConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString(),
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                PasswordHash = hasher.HashPassword(null, "Aa0_aa")
            }
        );
        // Associar este utilizador � role ADMIN
        modelBuilder.Entity<IdentityUserRole<string>>().HasData(
            new IdentityUserRole<string> { UserId = "admin", RoleId = "a" });
        
    }

public DbSet<Restaurantes> Restaurantes { get; set; } = default!;

public DbSet<Mesas> Mesas { get; set; } = default!;

public DbSet<Reservas> Reservas { get; set; } = default!;

public DbSet<Clientes> Clientes { get; set; } = default!;

public DbSet<Pagamento> Pagamento { get; set; } = default!;
}