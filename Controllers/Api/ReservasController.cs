using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Reserva_Restaurantes.Data;
using Reserva_Restaurantes.Models;

namespace Reserva_Restaurantes.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ReservasController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ReservasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Reservas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Reservas>>> GetReservas()
        {
            var userEmail = User.Identity.Name;

            if (User.IsInRole("Administrador"))
            {
                // Admin vê todas reservas
                return await _context.Reservas
                    .Include(r => r.Cliente)
                    .Include(r => r.Restaurante)
                    .ToListAsync();
            }

            if (User.IsInRole("Funcionario"))
            {
                var funcionario = await _context.Clientes.FirstOrDefaultAsync(c => c.Email == userEmail);
                if (funcionario == null) return Unauthorized();

                var restauranteId = funcionario.RestauranteFK;

                var reservasFuncionario = await _context.Reservas
                    .Include(r => r.Cliente)
                    .Include(r => r.Restaurante)
                    .Where(r => r.RestauranteFK == restauranteId)
                    .ToListAsync();

                return reservasFuncionario;
            }

            // Cliente vê só as suas reservas
            var cliente = await _context.Clientes.FirstOrDefaultAsync(c => c.Email == userEmail);
            if (cliente == null) return Unauthorized();

            var reservas = await _context.Reservas
                .Include(r => r.Restaurante)
                .Where(r => r.ClienteFK == cliente.Id)
                .ToListAsync();

            return reservas;
        }

        // GET: api/Reservas/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Reservas>> GetReserva(int id)
        {
            var reserva = await _context.Reservas
                .Include(r => r.Cliente)
                .Include(r => r.Restaurante)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (reserva == null) return NotFound();

            var userEmail = User.Identity.Name;

            if (User.IsInRole("Funcionario"))
            {
                var funcionario = await _context.Clientes.FirstOrDefaultAsync(c => c.Email == userEmail);
                if (funcionario == null || reserva.RestauranteFK != funcionario.RestauranteFK)
                    return Forbid();
            }
            else if (User.IsInRole("Administrador"))
            {
                // Admin pode ver tudo, ok
            }
            else
            {
                // Cliente só vê a sua reserva
                var cliente = await _context.Clientes.FirstOrDefaultAsync(c => c.Email == userEmail);
                if (cliente == null || reserva.ClienteFK != cliente.Id)
                    return Forbid();
            }

            return reserva;
        }

        // POST: api/Reservas
        [HttpPost]
        public async Task<ActionResult<Reservas>> PostReserva([FromBody] Reservas reserva)
        {
            var userEmail = User.Identity.Name;

            if (User.IsInRole("Funcionario"))
            {
                var funcionario = await _context.Clientes.FirstOrDefaultAsync(c => c.Email == userEmail);
                if (funcionario == null || reserva.RestauranteFK != funcionario.RestauranteFK)
                    return Forbid();
            }
            else if (User.IsInRole("Administrador"))
            {
                // Admin pode criar para qualquer restaurante
            }
            else
            {
                // Cliente só pode criar para si próprio (forçar ClienteFK)
                var cliente = await _context.Clientes.FirstOrDefaultAsync(c => c.Email == userEmail);
                if (cliente == null) return Unauthorized();

                reserva.ClienteFK = cliente.Id;
            }

            _context.Reservas.Add(reserva);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetReserva), new { id = reserva.Id }, reserva);
        }

        // PUT: api/Reservas/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Funcionario,Administrador")]
        public async Task<IActionResult> PutReserva(int id, [FromBody] Reservas reserva)
        {
            if (id != reserva.Id)
            {
                return BadRequest();
            }

            var existing = await _context.Reservas.AsNoTracking().FirstOrDefaultAsync(r => r.Id == id);
            if (existing == null) return NotFound();

            var userEmail = User.Identity.Name;

            if (User.IsInRole("Funcionario"))
            {
                var funcionario = await _context.Clientes.FirstOrDefaultAsync(c => c.Email == userEmail);
                if (funcionario == null || existing.RestauranteFK != funcionario.RestauranteFK)
                    return Forbid();
            }
            // Admin pode atualizar qualquer reserva

            _context.Entry(reserva).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Reservas.Any(e => e.Id == id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        // DELETE: api/Reservas/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Funcionario,Administrador")]
        public async Task<IActionResult> DeleteReserva(int id)
        {
            var reserva = await _context.Reservas.FindAsync(id);
            if (reserva == null) return NotFound();

            var userEmail = User.Identity.Name;

            if (User.IsInRole("Funcionario"))
            {
                var funcionario = await _context.Clientes.FirstOrDefaultAsync(c => c.Email == userEmail);
                if (funcionario == null || reserva.RestauranteFK != funcionario.RestauranteFK)
                    return Forbid();
            }
            // Admin pode apagar qualquer reserva

            _context.Reservas.Remove(reserva);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
