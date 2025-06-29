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

namespace Reserva_Restaurantes.Controllers
{
    [Authorize(Roles = "Funcionario,Administrador")]
    public class MesasController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MesasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Mesas
        public async Task<IActionResult> Index()
        {
            if (User.IsInRole("Funcionario"))
            {
                var userEmail = User.Identity?.Name;
                var funcionario = await _context.Clientes.FirstOrDefaultAsync(c => c.Email == userEmail);

                if (funcionario == null)
                    return Forbid();

                // Apenas mesas do restaurante do funcionário
                var mesasFuncionario = await _context.Mesas
                    .Include(m => m.Restaurante)
                    .Where(m => m.RestauranteFK == funcionario.RestauranteFK)
                    .ToListAsync();

                return View(mesasFuncionario);
            }
            
            var applicationDbContext =
                _context.Mesas.Include(m => m.Restaurante);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Mesas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mesas = await _context.Mesas
                .Include(m => m.Restaurante)
                .FirstOrDefaultAsync(m => m.Id == id);
            
            if (mesas == null)
            {
                return NotFound();
            }
            
            if (User.IsInRole("Funcionario"))
            {
                var userEmail = User.Identity?.Name;
                var funcionario = await _context.Clientes.FirstOrDefaultAsync(c => c.Email == userEmail);
                if (funcionario == null || funcionario.RestauranteFK != mesas.RestauranteFK)
                    return Forbid();
            }

            return View(mesas);
        }

        // GET: Mesas/Create
        public IActionResult Create()
        {
            ViewData["RestauranteFK"] = new SelectList(_context.Restaurantes, "Id", "Nome");
            return View();
        }
        
        // POST: Mesas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,NumMesa,Capacidade,RestauranteFK")] Mesas mesas)
        {
            bool mesaRepetida = await _context.Mesas
                .AnyAsync(m => m.NumMesa == mesas.NumMesa && m.RestauranteFK == mesas.RestauranteFK);

            if (mesaRepetida)
            {
                ModelState.AddModelError("", "Já existe uma mesa com este número neste restaurante.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(mesas);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["RestauranteFK"] = new SelectList(_context.Restaurantes, "Id", "Nome", mesas.RestauranteFK);
            return View(mesas);
        }


        // GET: Mesas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mesas = await _context.Mesas
                .Include(m => m.Restaurante)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (mesas == null)
            {
                return NotFound();
            }
            
            if (User.IsInRole("Funcionario"))
            {
                var userEmail = User.Identity?.Name;
                var funcionario = await _context.Clientes.FirstOrDefaultAsync(c => c.Email == userEmail);
                if (funcionario == null || funcionario.RestauranteFK != mesas.RestauranteFK)
                    return Forbid();
            }
            
            ViewData["RestauranteFK"] = new SelectList(_context.Restaurantes, "Id", "Nome");
            return View(mesas);
        }

        // POST: Mesas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,NumMesa,Capacidade,RestauranteFK")] Mesas mesas)
        {
            if (id != mesas.Id) return NotFound();
            
            if (User.IsInRole("Funcionario"))
            {
                var userEmail = User.Identity?.Name;
                var funcionario = await _context.Clientes.FirstOrDefaultAsync(c => c.Email == userEmail);
                if (funcionario == null || funcionario.RestauranteFK != mesas.RestauranteFK)
                    return Forbid();
            }

            bool mesaRepetida = await _context.Mesas
                .AnyAsync(m => m.NumMesa == mesas.NumMesa &&
                               m.RestauranteFK == mesas.RestauranteFK &&
                               m.Id != mesas.Id); // ignora a própria mesa

            if (mesaRepetida)
            {
                ModelState.AddModelError("", "Já existe uma mesa com este número neste restaurante.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(mesas);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MesasExists(mesas.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["RestauranteFK"] = new SelectList(_context.Restaurantes, "Id", "Nome", mesas.RestauranteFK);
            return View(mesas);
        }


        // GET: Mesas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mesas = await _context.Mesas
                .Include(m => m.Restaurante)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (mesas == null)
            {
                return NotFound();
            }
            
            if (User.IsInRole("Funcionario"))
            {
                var userEmail = User.Identity?.Name;
                var funcionario = await _context.Clientes.FirstOrDefaultAsync(c => c.Email == userEmail);
                if (funcionario == null || funcionario.RestauranteFK != mesas.RestauranteFK)
                    return Forbid();
            }
            
            ViewData["RestauranteFK"] = new SelectList(_context.Restaurantes, "Id", "Nome");
            return View(mesas);
        }

        // POST: Mesas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var mesas = await _context.Mesas.FindAsync(id);
            if (mesas != null)
            {
                if (User.IsInRole("Funcionario"))
                {
                    var userEmail = User.Identity?.Name;
                    var funcionario = await _context.Clientes.FirstOrDefaultAsync(c => c.Email == userEmail);
                    if (funcionario == null || funcionario.RestauranteFK != mesas.RestauranteFK)
                        return Forbid();
                }
                _context.Mesas.Remove(mesas);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MesasExists(int id)
        {
            return _context.Mesas.Any(e => e.Id == id);
        }
    }
}
