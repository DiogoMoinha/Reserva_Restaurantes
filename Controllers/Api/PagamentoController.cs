using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Reserva_Restaurantes.Data;
using Reserva_Restaurantes.Models;

namespace Reserva_Restaurantes.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PagamentoController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PagamentoController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Pagamento
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Pagamento>>> GetPagamentos()
        {
            var userEmail = User.Identity.Name;

            var query = _context.Pagamento
                .Include(p => p.Reserva)
                .ThenInclude(r => r.Cliente)
                .AsQueryable();

            if (User.IsInRole("Administrador"))
            {
                return await query.ToListAsync();
            }
            
            if (User.IsInRole("Funcionario"))
            {
                var funcionario = await _context.Clientes.FirstOrDefaultAsync(c => c.Email == userEmail);
                if (funcionario == null)
                    return Unauthorized();

                var restauranteId = funcionario.RestauranteFK;

                query = query.Where(p => p.Reserva.RestauranteFK == restauranteId);
                return await query.ToListAsync();
            }

            // Cliente normal
            var cliente = await _context.Clientes.FirstOrDefaultAsync(c => c.Email == userEmail);
            if (cliente == null)
                return Unauthorized();

            query = query.Where(p => p.Reserva.ClienteFK == cliente.Id);
            return await query.ToListAsync();
        }

        // GET: api/Pagamento/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Pagamento>> GetPagamento(int id)
        {
            var pagamento = await _context.Pagamento
                .Include(p => p.Reserva)
                .ThenInclude(r => r.Restaurante)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (pagamento == null)
                return NotFound();

            var userEmail = User.Identity.Name;
            var utilizador = await _context.Clientes.FirstOrDefaultAsync(c => c.Email == userEmail);
            if (utilizador == null) return Forbid();

            if (User.IsInRole("Administrador"))
            {
                return pagamento;
            }

            if (User.IsInRole("Funcionario"))
            {
                if (utilizador.RestauranteFK == pagamento.Reserva.RestauranteFK)
                    return pagamento;
                return Forbid();
            }

            if (pagamento.Reserva.ClienteFK == utilizador.Id)
                return pagamento;

            return Forbid();
        }

        // POST: api/Pagamento
        [HttpPost]
        [Authorize(Roles = "Funcionario")]
        public async Task<ActionResult<Pagamento>> CreatePagamento([FromBody] Pagamento pagamento)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Validar se o Funcionario pode criar para aquele restaurante
            var userEmail = User.Identity.Name;
            var funcionario = await _context.Clientes.FirstOrDefaultAsync(c => c.Email == userEmail);
            if (funcionario == null)
                return Unauthorized();

            var reserva = await _context.Reservas.FindAsync(pagamento.ReservasFK);
            if (reserva == null || reserva.RestauranteFK != funcionario.RestauranteFK)
                return Forbid();

            _context.Pagamento.Add(pagamento);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPagamento), new { id = pagamento.Id }, pagamento);
        }

        // PUT: api/Pagamento/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Funcionario")]
        public async Task<IActionResult> EditPagamento(int id, [FromBody] Pagamento pagamento)
        {
            if (id != pagamento.Id)
                return BadRequest("Id mismatch");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existing = await _context.Pagamento
                .Include(p => p.Reserva)
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id);

            if (existing == null)
                return NotFound();

            var userEmail = User.Identity.Name;
            var funcionario = await _context.Clientes.FirstOrDefaultAsync(c => c.Email == userEmail);
            if (funcionario == null || existing.Reserva.RestauranteFK != funcionario.RestauranteFK)
                return Forbid();

            _context.Entry(pagamento).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Pagamento.Any(e => e.Id == id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        // DELETE: api/Pagamento/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Funcionario")]
        public async Task<IActionResult> DeletePagamento(int id)
        {
            var pagamento = await _context.Pagamento
                .Include(p => p.Reserva)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (pagamento == null)
                return NotFound();

            var userEmail = User.Identity.Name;
            var funcionario = await _context.Clientes.FirstOrDefaultAsync(c => c.Email == userEmail);
            if (funcionario == null || pagamento.Reserva.RestauranteFK != funcionario.RestauranteFK)
                return Forbid();

            _context.Pagamento.Remove(pagamento);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
