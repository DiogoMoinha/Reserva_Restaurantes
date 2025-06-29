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
        // 'importa' todo o comportamento do método, 
        // aquando da sua definição na SuperClasse
        base.OnModelCreating(modelBuilder);

        // criar os perfis de utilizador da nossa app
        modelBuilder.Entity<IdentityRole>().HasData(
            new IdentityRole { Id = "a", Name = "Administrador", NormalizedName = "ADMINISTRADOR" });
        modelBuilder.Entity<IdentityRole>().HasData(
            new IdentityRole { Id = "f", Name = "Funcionario", NormalizedName = "FUNCIONARIO" });

        // criar um utilizador para funcionar como ADMIN
        // função para codificar a password
        var hasher = new PasswordHasher<IdentityUser>();
        // criação do utilizador
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
        // Associar este utilizador à role ADMIN
        modelBuilder.Entity<IdentityUserRole<string>>().HasData(
            new IdentityUserRole<string> { UserId = "admin", RoleId = "a" });
        modelBuilder.Entity<Reserva_Mesa>()
            .HasKey(rm => new { rm.ReservasFK, rm.MesasFK });
        modelBuilder.Entity<Reserva_Mesa>()
            .HasOne(rm => rm.Reservas)
            .WithMany(r => r.ReservasMesas)
            .HasForeignKey(rm => rm.ReservasFK)
            .OnDelete(DeleteBehavior.Restrict); // ? evitar múltiplos cascades

        modelBuilder.Entity<Reserva_Mesa>()
            .HasOne(rm => rm.Mesas)
            .WithMany(m => m.ReservasMesas)
            .HasForeignKey(rm => rm.MesasFK)
            .OnDelete(DeleteBehavior.Restrict); // idem
    }

public DbSet<Restaurantes> Restaurantes { get; set; } = default!;

public DbSet<Mesas> Mesas { get; set; } = default!;

public DbSet<Reservas> Reservas { get; set; } = default!;

public DbSet<Clientes> Clientes { get; set; } = default!;

public DbSet<Pagamento> Pagamento { get; set; } = default!;
}