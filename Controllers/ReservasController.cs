using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Reserva_Restaurantes.Data;
using Reserva_Restaurantes.Models;

namespace Reserva_Restaurantes.Controllers
{
    [Authorize]
    public class ReservasController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReservasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Reservas
        public async Task<IActionResult> Index()
        {
            // Obter o email do utilizador autenticado
            var userEmail = User.Identity.Name;

            if (User.IsInRole("Administrador"))
            {
                var applicationDbContext = _context.Reservas
                    .Include(p => p.Cliente)
                    .Include(r => r.Restaurante);
                // Admins and Funcionarios can see all payments
                return View(await applicationDbContext.ToListAsync());
            }

            if (User.IsInRole("Funcionario"))
            {
                // Obter o restaurante associado ao funcionário (cliente)
                var funcionario = await _context.Clientes
                    .FirstOrDefaultAsync(c => c.Email == userEmail);

                if (funcionario == null)
                {
                    return Unauthorized(); // ou RedirectToAction("AccessDenied")
                }

                var restauranteId = funcionario.RestauranteFK;

                // Apenas reservas do restaurante do funcionário
                var reservasFuncionario = _context.Reservas
                    .Include(r => r.Restaurante)
                    .Include(r => r.Cliente)
                    .Where(r => r.RestauranteFK == restauranteId);

                return View(await reservasFuncionario.ToListAsync());
            }

            // Procurar o cliente correspondente ao email do utilizador
            var cliente = await _context.Clientes
                .FirstOrDefaultAsync(c => c.Email == userEmail);

            // Filtrar as reservas do cliente
            var reservas = await _context.Reservas
                .Include(r => r.Restaurante)
                .Where(r => r.ClienteFK == cliente.Id)
                .ToListAsync();

            return View(reservas);
        }

        // GET: Reservas/Details/5
        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservas = await _context.Reservas
                .Include(r => r.Restaurante)
                .Include(r => r.Cliente)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (reservas == null)
            {
                return NotFound();
            }

            var userEmail = User.Identity.Name;
            if (User.IsInRole("Funcionario"))
            {
                var funcionario = await _context.Clientes.FirstOrDefaultAsync(c => c.Email == userEmail);
                if (funcionario == null || reservas.RestauranteFK != funcionario.RestauranteFK)
                    return Forbid();
            }

            var utilizador = await _context.Clientes.FirstOrDefaultAsync(c => c.Email == userEmail);
            if (utilizador == null) return Forbid();
            if (reservas.ClienteFK == utilizador.Id)
            {
                return View(reservas);
            }

            ViewData["ClienteFK"] = new SelectList(_context.Clientes, "Id", "Nome");
            ViewData["RestauranteFK"] = new SelectList(_context.Restaurantes, "Id", "Nome");
            return Forbid();
        }

        // GET: Reservas/Create
        [Authorize]
        public IActionResult Create()
        {
            ViewData["ClienteFK"] = new SelectList(_context.Clientes, "Id", "Nome");
            ViewData["RestauranteFK"] = new SelectList(_context.Restaurantes, "Id", "Nome");
            return View();
        }

        // POST: Reservas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("Id,Data,Hora,PessoasQtd,ClienteFK,RestauranteFK")]
            Reservas reservas)
        {
            if (ModelState.IsValid)
            {
                _context.Add(reservas);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["ClienteFK"] = new SelectList(_context.Clientes, "Id", "Nome", reservas.ClienteFK);
            ViewData["RestauranteFK"] = new SelectList(_context.Restaurantes, "Id", "Nome", reservas.RestauranteFK);
            return View(reservas);
        }

        // GET: Reservas/Edit/5
        [Authorize(Roles = "Funcionario")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reserva = await _context.Reservas
                .Include(r => r.Restaurante)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (reserva == null) return NotFound();

            if (User.IsInRole("Funcionario"))
            {
                var userEmail = User.Identity.Name;
                var funcionario = await _context.Clientes.FirstOrDefaultAsync(c => c.Email == userEmail);
                if (funcionario == null) return Unauthorized();

                if (reserva.RestauranteFK != funcionario.RestauranteFK)
                    return Forbid();
            }

            ViewData["ClienteFK"] = new SelectList(_context.Clientes, "Id", "Nome");
            ViewData["RestauranteFK"] = new SelectList(_context.Restaurantes, "Id", "Nome");
            return View(reserva);
        }

        // POST: Reservas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Funcionario")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,
            [Bind("Id,Data,Hora,PessoasQtd,ClienteFK,RestauranteFK")]
            Reservas reservas)
        {
            if (id != reservas.Id)
            {
                return NotFound();
            }

            var existing = await _context.Reservas
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == id);

            if (existing == null) return NotFound();

            if (User.IsInRole("Funcionario"))
            {
                var userEmail = User.Identity.Name;
                var funcionario = await _context.Clientes.FirstOrDefaultAsync(c => c.Email == userEmail);
                if (funcionario == null || existing.RestauranteFK != funcionario.RestauranteFK)
                    return Forbid();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(reservas);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReservasExists(reservas.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return RedirectToAction(nameof(Index));
            }

            ViewData["ClienteFK"] = new SelectList(_context.Clientes, "Id", "Nome", reservas.ClienteFK);
            ViewData["RestauranteFK"] = new SelectList(_context.Restaurantes, "Id", "Nome", reservas.RestauranteFK);
            return View(reservas);
        }

        // GET: Reservas/Delete/5
        [Authorize(Roles = "Funcionario")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservas = await _context.Reservas
                .Include(r => r.Restaurante)
                .Include(r => r.Cliente)
                .FirstOrDefaultAsync(r => r.Id == id);
            if (reservas == null)
            {
                return NotFound();
            }

            if (User.IsInRole("Funcionario"))
            {
                var userEmail = User.Identity.Name;
                var funcionario = await _context.Clientes.FirstOrDefaultAsync(c => c.Email == userEmail);
                if (funcionario == null || reservas.RestauranteFK != funcionario.RestauranteFK)
                    return Forbid();
            }

            ViewData["ClienteFK"] = new SelectList(_context.Clientes, "Id", "Nome");
            ViewData["RestauranteFK"] = new SelectList(_context.Restaurantes, "Id", "Nome");
            return View(reservas);
        }

        // POST: Reservas/Delete/5
        [Authorize(Roles = "Funcionario")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var reservas = await _context.Reservas.FindAsync(id);


            if (reservas != null)
            {
                if (User.IsInRole("Funcionario"))
                {
                    var userEmail = User.Identity.Name;
                    var funcionario = await _context.Clientes.FirstOrDefaultAsync(c => c.Email == userEmail);
                    if (funcionario == null || reservas.RestauranteFK != funcionario.RestauranteFK)
                        return Forbid();
                }

                _context.Reservas.Remove(reservas);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReservasExists(int id)
        {
            return _context.Reservas.Any(e => e.Id == id);
        }

        // GET: Reservas/Confirmar/5
        [Authorize(Roles = "Funcionario")]
        public async Task<IActionResult> Confirmar(int? id)
        {
            if (id == null) return NotFound();

            var reserva = await _context.Reservas
                .Include(r => r.Cliente)
                .Include(r => r.Restaurante)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (reserva == null) return NotFound();
            
            // Garante que Cliente não é null
            if (reserva.Cliente == null)
            {
                // Pode lançar exceção, ou mostrar erro, ou preencher valor padrão
                return BadRequest("Cliente da reserva não encontrado.");
            }

            var userEmail = User.Identity?.Name;
            var funcionario = await _context.Clientes.FirstOrDefaultAsync(c => c.Email == userEmail);

            if (funcionario == null || funcionario.RestauranteFK != reserva.RestauranteFK)
                return Forbid();

            // Carregar mesas completas, incluindo capacidade
            var mesas = await _context.Mesas
                .Where(m => m.RestauranteFK == reserva.RestauranteFK)
                .ToListAsync();

            ViewBag.Mesas = mesas;

            return View(reserva);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Funcionario")]
        public async Task<IActionResult> Confirmar(int id, List<int> mesasSelecionadas)
        {
            var reserva = await _context.Reservas
                .Include(r => r.ReservasMesas)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (reserva == null) return NotFound();

            var userEmail = User.Identity?.Name;
            var funcionario = await _context.Clientes.FirstOrDefaultAsync(c => c.Email == userEmail);

            if (funcionario == null || funcionario.RestauranteFK != reserva.RestauranteFK)
                return Forbid();

            if (mesasSelecionadas == null || !mesasSelecionadas.Any())
                ModelState.AddModelError("", "Selecione pelo menos uma mesa.");

            var mesasValidas = await _context.Mesas
                .Where(m => mesasSelecionadas.Contains(m.Id) && m.RestauranteFK == funcionario.RestauranteFK)
                .ToListAsync();

            if (mesasValidas.Count != mesasSelecionadas.Count)
                ModelState.AddModelError("", "Algumas mesas selecionadas não pertencem ao seu restaurante.");

            int capacidadeTotal = mesasValidas.Sum(m => m.Capacidade);
            if (capacidadeTotal < reserva.PessoasQtd)
                ModelState.AddModelError("",
                    $"Capacidade total das mesas selecionadas ({capacidadeTotal}) é inferior ao número de pessoas da reserva ({reserva.PessoasQtd}).");

            // Calcular intervalo para conflitos
            var dataReserva = reserva.Data;
            var horaReserva = reserva.Hora.TimeOfDay;

            TimeSpan horaInicioJanela = horaReserva.Subtract(TimeSpan.FromHours(1));
            DateTime dataInicio = dataReserva;
            if (horaInicioJanela < TimeSpan.Zero)
            {
                horaInicioJanela = horaInicioJanela.Add(TimeSpan.FromHours(24)); // Ajustar para o dia anterior
                dataInicio = dataInicio.AddDays(-1);
            }

            var conflitos = await _context.Reservas
                .Where(r =>
                    r.Confirmada &&
                    r.Id != reserva.Id &&
                    r.RestauranteFK == funcionario.RestauranteFK &&
                    (
                        (r.Data == dataInicio && r.Hora.TimeOfDay >= horaInicioJanela) ||
                        (r.Data == dataReserva && r.Hora.TimeOfDay < horaReserva)
                    )
                )
                .Include(r => r.ReservasMesas)
                .ToListAsync();

            var mesasOcupadas = conflitos
                .SelectMany(r => r.ReservasMesas.Select(rm => rm.MesasFK))
                .Distinct()
                .ToList();

            if (mesasSelecionadas.Any(m => mesasOcupadas.Contains(m)))
            {
                ModelState.AddModelError("",
                    "Algumas mesas selecionadas estão ocupadas por reservas confirmadas até 1 hora antes desta reserva.");
            }

            if (!ModelState.IsValid)
            {
                var mesas = await _context.Mesas
                    .Where(m => m.RestauranteFK == funcionario.RestauranteFK)
                    .ToListAsync();

                ViewBag.Mesas = mesas;
                
                // Recarregar Cliente para a reserva antes de retornar a View
                reserva = await _context.Reservas
                    .Include(r => r.Cliente)
                    .Include(r => r.Restaurante)
                    .FirstOrDefaultAsync(r => r.Id == reserva.Id);
                
                return View(reserva);
            }

            // Limpar associações anteriores
            reserva.ReservasMesas.Clear();

            foreach (var mesa in mesasValidas)
            {
                reserva.ReservasMesas.Add(new Reserva_Mesa
                {
                    MesasFK = mesa.Id,
                    ReservasFK = reserva.Id
                });
            }

            reserva.Confirmada = true;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}