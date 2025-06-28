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
        var applicationDbContext = _context.Pagamento.Include(p => p.Reserva);
        return View(await applicationDbContext.ToListAsync());
    }

    // GET: Pagamento/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
            return NotFound();

        var pagamento = await _context.Pagamento
            .Include(p => p.Reserva)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (pagamento == null)
            return NotFound();

        return View(pagamento);
    }

    // GET: Pagamento/Create
    [Authorize(Roles = "Funcionario")]
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

        var pagamento = await _context.Pagamento.FindAsync(id);
        if (pagamento == null)
            return NotFound();

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
            .FirstOrDefaultAsync(m => m.Id == id);

        if (pagamento == null)
            return NotFound();

        return View(pagamento);
    }

    // POST: Pagamento/Delete/5
    [Authorize(Roles = "Funcionario")]
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var pagamento = await _context.Pagamento.FindAsync(id);
        if (pagamento != null)
        {
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
