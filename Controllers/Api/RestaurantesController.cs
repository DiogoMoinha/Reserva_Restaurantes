using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Reserva_Restaurantes.Data;
using Reserva_Restaurantes.Models;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Reserva_Restaurantes.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class RestaurantesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public RestaurantesController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: api/Restaurantes
        [HttpGet]
        public async Task<ActionResult> GetRestaurantes()
        {
            var restaurantes = await _context.Restaurantes.ToListAsync();
            return Ok(restaurantes);
        }

        // GET: api/Restaurantes/5
        [HttpGet("{id}")]
        public async Task<ActionResult> GetRestaurante(int id)
        {
            var restaurante = await _context.Restaurantes.FindAsync(id);
            if (restaurante == null)
                return NotFound();

            return Ok(restaurante);
        }

        // POST: api/Restaurantes
        [Authorize(Roles = "Administrador")]
        [HttpPost]
        public async Task<ActionResult> CreateRestaurante([FromForm] Restaurantes restaurantes, IFormFile imagemFoto)
        {
            bool haErro = false;
            string nomeImagem = "";

            bool existeNome = await _context.Restaurantes.AnyAsync(r => r.Nome == restaurantes.Nome);
            if (existeNome)
            {
                ModelState.AddModelError("Nome", "Já existe um restaurante com este nome.");
                haErro = true;
            }

            if (imagemFoto == null)
            {
                haErro = true;
                ModelState.AddModelError("Foto", "Tem de submeter uma Fotografia");
            }
            else if (imagemFoto.ContentType != "image/jpeg" && imagemFoto.ContentType != "image/png")
            {
                haErro = true;
                ModelState.AddModelError("Foto", "A fotografia deve ser jpeg ou png.");
            }
            else
            {
                Guid g = Guid.NewGuid();
                nomeImagem = g.ToString() + Path.GetExtension(imagemFoto.FileName).ToLowerInvariant();
                restaurantes.Foto = nomeImagem;
            }

            ModelState.Remove("Foto");

            if (!ModelState.IsValid || haErro)
            {
                return BadRequest(ModelState);
            }

            _context.Restaurantes.Add(restaurantes);
            await _context.SaveChangesAsync();

            // Guardar imagem no disco
            var localizacaoImagem = Path.Combine(_webHostEnvironment.WebRootPath, "imagens");
            if (!Directory.Exists(localizacaoImagem))
                Directory.CreateDirectory(localizacaoImagem);

            var caminhoCompleto = Path.Combine(localizacaoImagem, nomeImagem);
            using var stream = new FileStream(caminhoCompleto, FileMode.Create);
            await imagemFoto.CopyToAsync(stream);

            return CreatedAtAction(nameof(GetRestaurante), new { id = restaurantes.Id }, restaurantes);
        }

        // PUT: api/Restaurantes/5
        [Authorize(Roles = "Administrador,Funcionario")]
        [HttpPut("{id}")]
        public async Task<ActionResult> EditRestaurante(int id, [FromForm] Restaurantes restaurantes, IFormFile? imagemFoto)
        {
            if (id != restaurantes.Id)
                return BadRequest();

            // Se for funcionário, validar associação ao restaurante
            if (User.IsInRole("Funcionario"))
            {
                var userEmail = User.Identity.Name;
                var funcionario = await _context.Clientes.FirstOrDefaultAsync(c => c.Email == userEmail);
                if (funcionario == null || funcionario.RestauranteFK != restaurantes.Id)
                    return Forbid();
            }

            var restauranteOriginal = await _context.Restaurantes.AsNoTracking().FirstOrDefaultAsync(r => r.Id == id);
            if (restauranteOriginal == null)
                return NotFound();

            bool existeNome = await _context.Restaurantes
                .AnyAsync(r => r.Nome == restaurantes.Nome && r.Id != restaurantes.Id);

            if (existeNome)
                return BadRequest(new { error = "Já existe um restaurante com este nome." });

            if (imagemFoto == null)
            {
                restaurantes.Foto = restauranteOriginal.Foto;
            }
            else
            {
                if (imagemFoto.ContentType != "image/jpeg" && imagemFoto.ContentType != "image/png")
                    return BadRequest(new { error = "A fotografia deve ser jpeg ou png." });

                string nomeImagem = Guid.NewGuid().ToString() + Path.GetExtension(imagemFoto.FileName).ToLowerInvariant();
                restaurantes.Foto = nomeImagem;

                var localizacaoImagem = Path.Combine(_webHostEnvironment.WebRootPath, "imagens");
                if (!Directory.Exists(localizacaoImagem))
                    Directory.CreateDirectory(localizacaoImagem);

                string caminhoCompleto = Path.Combine(localizacaoImagem, nomeImagem);
                using var stream = new FileStream(caminhoCompleto, FileMode.Create);
                await imagemFoto.CopyToAsync(stream);
            }

            try
            {
                _context.Restaurantes.Update(restaurantes);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RestaurantesExists(id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        // DELETE: api/Restaurantes/5
        [Authorize(Roles = "Administrador,Funcionario")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteRestaurante(int id)
        {
            var restaurante = await _context.Restaurantes.FindAsync(id);
            if (restaurante == null)
                return NotFound();

            if (User.IsInRole("Funcionario"))
            {
                var userEmail = User.Identity.Name;
                var funcionario = await _context.Clientes.FirstOrDefaultAsync(c => c.Email == userEmail);
                if (funcionario == null || funcionario.RestauranteFK != restaurante.Id)
                    return Forbid();
            }

            _context.Restaurantes.Remove(restaurante);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool RestaurantesExists(int id)
        {
            return _context.Restaurantes.Any(e => e.Id == id);
        }
    }
}
