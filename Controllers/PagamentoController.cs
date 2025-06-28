using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Reserva_Restaurantes.Data;
using Reserva_Restaurantes.Models;

[Authorize]
public class PagamentoController : Controller
{
    
    private readonly ApplicationDbContext _context;
    
    public PagamentoController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: Pagamento
    public async Task<IActionResult> Index()
    {
        var userEmail = User.Identity.Name;
        
        var applicationDbContext = _context.Pagamento
            .Include(p => p.Reserva)
            .ThenInclude(r => r.Cliente);
        
        if (User.IsInRole("Administrador"))
        {
            // Admins can see all payments
            return View(await applicationDbContext.ToListAsync());
        }
        
        if (User.IsInRole("Funcionario"))
        {
            // Obter o funcionário pelo email
            var funcionario = await _context.Clientes.FirstOrDefaultAsync(c => c.Email == userEmail);
            if (funcionario == null)
                return Unauthorized();

            var restauranteId = funcionario.RestauranteFK;

            // Filtrar pagamentos para os que pertencem ao restaurante do funcionário
            var pagamentosFuncionario = _context.Pagamento
                .Include(p => p.Reserva)
                .ThenInclude(r => r.Restaurante)
                .Where(p => p.Reserva.RestauranteFK == restauranteId);

            return View(await pagamentosFuncionario.ToListAsync());
        }
        
        // For regular users (Clientes)
        if (string.IsNullOrEmpty(userEmail))
            return Unauthorized();

        var cliente = await _context.Clientes
            .FirstOrDefaultAsync(c => c.Email == userEmail);

        if (cliente == null)
            return NotFound("Cliente não encontrado");

        // Only show payments related to the client's reservations
        var userPagamentos = await applicationDbContext
            .Where(p => p.Reserva.ClienteFK == cliente.Id)
            .ToListAsync();

        return View(userPagamentos);
    }

    // GET: Pagamento/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
            return NotFound();

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
            return View(pagamento);
        }

        if (User.IsInRole("Funcionario"))
        {
            if (utilizador.RestauranteFK == pagamento.Reserva.RestauranteFK)
                return View(pagamento);
        }

        if (pagamento.Reserva.ClienteFK == utilizador.Id)
        {
            return View(pagamento);
        }

        return Forbid();
    }

    // GET: Pagamento/Create
    [Authorize(Roles = "Administrador,Funcionario")]
    public IActionResult Create()
    {
        PopularViewData();
        return View();
    }

    // POST: Pagamento/Create
    [Authorize(Roles = "Funcionario")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,Metodo,Estado,ReservasFK")] Pagamento pagamento)
    {
        if (ModelState.IsValid)
        {
            _context.Add(pagamento);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        PopularViewData();
        return View(pagamento);
    }

    // GET: Pagamento/Edit/5
    [Authorize(Roles = "Funcionario")]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
            return NotFound();

        var pagamento = await _context.Pagamento
            .Include(p => p.Reserva)
            .FirstOrDefaultAsync(p => p.Id == id);
        if (pagamento == null)
            return NotFound();
        
        if (User.IsInRole("Funcionario"))
        {
            var userEmail = User.Identity.Name;
            var funcionario = await _context.Clientes.FirstOrDefaultAsync(c => c.Email == userEmail);
            if (funcionario == null || pagamento.Reserva.RestauranteFK != funcionario.RestauranteFK)
                return Forbid();
        }

        PopularViewData();
        return View(pagamento);
    }

    // POST: Pagamento/Edit/5
    [Authorize(Roles = "Funcionario")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Metodo,Estado,ReservasFK")] Pagamento pagamento)
    {
        if (id != pagamento.Id)
            return NotFound();
        
        var existing = await _context.Pagamento
            .Include(p => p.Reserva)
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id);

        if (existing == null) return NotFound();

        if (User.IsInRole("Funcionario"))
        {
            var userEmail = User.Identity.Name;
            var funcionario = await _context.Clientes.FirstOrDefaultAsync(c => c.Email == userEmail);
            if (funcionario == null || existing.Reserva.RestauranteFK != funcionario.RestauranteFK)
                return Forbid();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(pagamento);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PagamentoExists(pagamento.Id))
                    return NotFound();
                else
                    throw;
            }
            return RedirectToAction(nameof(Index));
        }
        PopularViewData();
        return View(pagamento);
    }

    // GET: Pagamento/Delete/5
    [Authorize(Roles = "Funcionario")]
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
            return NotFound();

        var pagamento = await _context.Pagamento
            .Include(p => p.Reserva)
            .ThenInclude(r => r.Restaurante)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (pagamento == null)
            return NotFound();
        
        if (User.IsInRole("Funcionario"))
        {
            var userEmail = User.Identity.Name;
            var funcionario = await _context.Clientes.FirstOrDefaultAsync(c => c.Email == userEmail);

            if (funcionario == null || pagamento.Reserva.RestauranteFK != funcionario.RestauranteFK)
                return Forbid();
        }

        return View(pagamento);
    }

    // POST: Pagamento/Delete/5
    [Authorize(Roles = "Funcionario")]
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var pagamento = await _context.Pagamento
            .Include(p => p.Reserva)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (pagamento != null)
        {
            if (User.IsInRole("Funcionario"))
            {
                var userEmail = User.Identity.Name;
                var funcionario = await _context.Clientes.FirstOrDefaultAsync(c => c.Email == userEmail);

                if (funcionario == null || pagamento.Reserva.RestauranteFK != funcionario.RestauranteFK)
                    return Forbid();
            }
            _context.Pagamento.Remove(pagamento);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }

    private bool PagamentoExists(int id)
    {
        return _context.Pagamento.Any(e => e.Id == id);
    }

    // Método auxiliar para popular ViewData com SelectLists para os dropdowns
    private void PopularViewData()
    {
        ViewBag.MetodosPagamento = new SelectList(Enum.GetValues(typeof(Pagamento.MetodoPagamento)), "");
        ViewBag.EstadosPagamento = new SelectList(Enum.GetValues(typeof(Pagamento.Estados)), "");
        ViewBag.Reservas = new SelectList(_context.Reservas, "Id", "Id");
        
    }
}
