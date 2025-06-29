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
    [Authorize(Roles = "Funcionario,Administrador")]
    [Route("api/[controller]")]
    [ApiController]
    public class MesasController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public MesasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/MesasAuth
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Mesas>>> GetMesas()
        {
            var query = _context.Mesas.Include(m => m.Restaurante).AsQueryable();

            if (User.IsInRole("Funcionario"))
            {
                var funcionario = await GetFuncionarioAsync();
                if (funcionario == null) return Forbid();

                query = query.Where(m => m.RestauranteFK == funcionario.RestauranteFK);
            }

            return await query.ToListAsync();
        }

        // GET: api/MesasAuth/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Mesas>> GetMesa(int id)
        {
            var mesa = await _context.Mesas.Include(m => m.Restaurante).FirstOrDefaultAsync(m => m.Id == id);

            if (mesa == null)
                return NotFound();

            if (User.IsInRole("Funcionario"))
            {
                var funcionario = await GetFuncionarioAsync();
                if (funcionario == null || funcionario.RestauranteFK != mesa.RestauranteFK)
                    return Forbid();
            }

            return mesa;
        }

        // POST: api/MesasAuth
        [HttpPost]
        public async Task<ActionResult<Mesas>> PostMesa(Mesas mesa)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (User.IsInRole("Funcionario"))
            {
                var funcionario = await GetFuncionarioAsync();
                if (funcionario == null || funcionario.RestauranteFK != mesa.RestauranteFK)
                    return Forbid();
            }

            bool mesaRepetida = await _context.Mesas.AnyAsync(m =>
                m.NumMesa == mesa.NumMesa && m.RestauranteFK == mesa.RestauranteFK);

            if (mesaRepetida)
                return Conflict("Já existe uma mesa com este número neste restaurante.");

            _context.Mesas.Add(mesa);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetMesa), new { id = mesa.Id }, mesa);
        }

        // PUT: api/MesasAuth/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMesa(int id, Mesas mesa)
        {
            if (id != mesa.Id)
                return BadRequest("ID na URL e no corpo não coincidem.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (User.IsInRole("Funcionario"))
            {
                var funcionario = await GetFuncionarioAsync();
                if (funcionario == null || funcionario.RestauranteFK != mesa.RestauranteFK)
                    return Forbid();
            }

            bool mesaRepetida = await _context.Mesas.AnyAsync(m =>
                m.NumMesa == mesa.NumMesa &&
                m.RestauranteFK == mesa.RestauranteFK &&
                m.Id != mesa.Id);

            if (mesaRepetida)
                return Conflict("Já existe uma mesa com este número neste restaurante.");

            _context.Entry(mesa).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MesaExists(id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        // DELETE: api/MesasAuth/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMesa(int id)
        {
            var mesa = await _context.Mesas.FindAsync(id);
            if (mesa == null)
                return NotFound();

            if (User.IsInRole("Funcionario"))
            {
                var funcionario = await GetFuncionarioAsync();
                if (funcionario == null || funcionario.RestauranteFK != mesa.RestauranteFK)
                    return Forbid();
            }

            _context.Mesas.Remove(mesa);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // Auxiliar
        private bool MesaExists(int id)
        {
            return _context.Mesas.Any(e => e.Id == id);
        }

        private async Task<Clientes?> GetFuncionarioAsync()
        {
            var email = User.Identity?.Name;
            return await _context.Clientes.FirstOrDefaultAsync(c => c.Email == email);
        }
    }
}
