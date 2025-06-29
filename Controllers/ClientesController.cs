using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Reserva_Restaurantes.Data;
using Reserva_Restaurantes.Models;

namespace Reserva_Restaurantes.Controllers
{
    [Authorize]
    public class ClientesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ClientesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Clientes
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Clientes.ToListAsync());
        }

        // GET: Clientes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            
            var cliente = _context.Clientes.FirstOrDefault(c => c.Id == id);
            if (cliente == null)
                return NotFound();
            
            if (!User.IsInRole("Administrador") && cliente.Email != User.Identity?.Name)
                return Forbid();

            return View(cliente);
        }

        // GET: Clientes/Create
        [Authorize(Roles = "Administrador")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Clientes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Create([Bind("Id,Nome,Email,Telefone")] Clientes cliente)
        {
            if (ModelState.IsValid)
            {
                _context.Add(cliente);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(cliente);
        }


        // GET: Clientes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var clientes = await _context.Clientes.FindAsync(id);
            if (clientes == null)
            {
                return NotFound();
            }
            
            if (!User.IsInRole("Administrador") && clientes.Email != User.Identity?.Name)
                return Forbid();
            
            return View(clientes);
        }

        // POST: Clientes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nome,Email,Telefone")] Clientes clientes)
        {
            if (id != clientes.Id)
            {
                return NotFound();
            }
            
            var clienteOriginal = await _context.Clientes.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);
            if (clienteOriginal == null)
                return NotFound();

            if (!User.IsInRole("Administrador") && clienteOriginal.Email != User.Identity?.Name)
                return Forbid();
            
            // Verificar se já existe outro cliente com o mesmo Email
            bool emailDuplicado = await _context.Clientes
                .AnyAsync(c => c.Email == clientes.Email && c.Id != clientes.Id);

            if (emailDuplicado)
                ModelState.AddModelError("Email", "Este email já está em uso.");
            
            // Verificar se já existe outro cliente com o mesmo Email
            bool NomeDuplicado = await _context.Clientes
                .AnyAsync(c => c.Nome == clientes.Nome && c.Id != clientes.Id);

            if (NomeDuplicado)
                ModelState.AddModelError("Nome", "Este nome já está em uso.");

            // Verificar se já existe outro cliente com o mesmo Telefone
            bool telefoneDuplicado = await _context.Clientes
                .AnyAsync(c => c.Telefone == clientes.Telefone && c.Id != clientes.Id);

            if (telefoneDuplicado)
                ModelState.AddModelError("Telefone", "Este número de telemóvel já está em uso.");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(clientes);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClientesExists(clientes.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Details), new { id = clientes.Id });
            }
            return View(clientes);
        }

        // GET: Clientes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var clientes = await _context.Clientes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (clientes == null)
            {
                return NotFound();
            }
            
            if (!User.IsInRole("Administrador") && clientes.Email != User.Identity?.Name)
                return Forbid();

            return View(clientes);
        }

        // POST: Clientes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var clientes = await _context.Clientes.FindAsync(id);
            if (clientes != null)
            {
                if (!User.IsInRole("Administrador") && clientes.Email != User.Identity?.Name)
                    return Forbid();
                _context.Clientes.Remove(clientes);
            }

            await _context.SaveChangesAsync();
            if(User.IsInRole("Administrador")){
                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction("Logout", "Account"); // logout após autodestruição
        }

        private bool ClientesExists(int id)
        {
            return _context.Clientes.Any(e => e.Id == id);
        }
        
        
        [Authorize]
        public IActionResult MyDetails()
        {
            var email = User.Identity?.Name;
            if (string.IsNullOrEmpty(email))
                return Unauthorized();

            var cliente = _context.Clientes.FirstOrDefault(c => c.Email == email);
            if (cliente == null)
                return NotFound();

            return RedirectToAction("Details", new { id = cliente.Id });
        }
        
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> PromoteToFuncionario(int id)
        {
            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente == null)
                return NotFound();

            var restauranteList = await _context.Restaurantes.ToListAsync();
            ViewBag.Restaurantes = new SelectList(restauranteList, "Id", "Nome");

            return View(cliente);
        }

        [Authorize(Roles = "Administrador")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PromoteToFuncionario(int id, int restauranteId, [FromServices] UserManager<IdentityUser> userManager)
        {
            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente == null)
                return NotFound();

            // Assign role
            var user = await userManager.FindByEmailAsync(cliente.Email);
            if (user == null)
                return NotFound("User not found in Identity.");

            if (!await userManager.IsInRoleAsync(user, "Funcionario"))
            {
                await userManager.AddToRoleAsync(user, "Funcionario");
            }

            // Associate with restaurante
            cliente.RestauranteFK = restauranteId;
            _context.Update(cliente);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
