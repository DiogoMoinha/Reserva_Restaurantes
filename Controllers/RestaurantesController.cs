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
    public class RestaurantesController : Controller
    {
        /// <summary>
        /// referência à Base de Dados
        /// </summary>
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// objeto que contém todas as características do Servidor
        /// </summary>
        private readonly IWebHostEnvironment _webHostEnvironment;
        
        public RestaurantesController(ApplicationDbContext context,IWebHostEnvironment webHostEnvironment) {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Restaurantes
        public async Task<IActionResult> Index()
        {
            return View(await _context.Restaurantes.ToListAsync());
        }

        // GET: Restaurantes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var restaurantes = await _context.Restaurantes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (restaurantes == null)
            {
                return NotFound();
            }

            return View(restaurantes);
        }

        [Authorize]
        // GET: Restaurantes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Restaurantes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nome,Endereco,CodPostal,HoraAbertura,HoraFecho,Foto")] Restaurantes restaurantes, IFormFile imagemFoto)
        {
            // vars. auxiliar
            bool haErro = false;
            string nomeImagem = "";
            
            // Verificar se já existe restaurante com o mesmo nome
            bool existeNome = await _context.Restaurantes.AnyAsync(r => r.Nome == restaurantes.Nome);
            if (existeNome)
            {
                ModelState.AddModelError("Nome", "Já existe um restaurante com este nome.");
                haErro = true;
            }
            
            if (imagemFoto == null) {
                // não há ficheiro
                haErro = true;
                // construo a msg de erro
                ModelState.AddModelError("", "Tem de submeter uma Fotografia");
            }
            else {
                // há ficheiro. Mas, é uma imagem?
                // https://developer.mozilla.org/en-US/docs/Web/HTTP/Guides/MIME_types
                if (imagemFoto.ContentType != "image/jpeg" && imagemFoto.ContentType != "image/png") {
                    // !(A==b || A==c) <=> (A!=b && A!=c)

                    // não há imagem
                    haErro = true;
                    // construo a msg de erro
                    ModelState.AddModelError("", "Tem de submeter uma Fotografia");
                }else {
                    // há imagem,
                    // vamos processá-la
                    // *******************************
                    // Novo nome para o ficheiro
                    Guid g = Guid.NewGuid();
                    nomeImagem = g.ToString();
                    string extensao = Path.GetExtension(imagemFoto.FileName).ToLowerInvariant();
                    nomeImagem += extensao;

                    // guardar este nome na BD
                    restaurantes.Foto = nomeImagem;
                }
            }
            // desligar a validação do atributo Foto
            ModelState.Remove("Foto");
            
            
            // Avalia se os dados estão de acordo com o Model
            if (ModelState.IsValid && !haErro) {
                _context.Add(restaurantes);
                await _context.SaveChangesAsync();
                
                // **********************************************
                // guardar o ficheiro no disco rígido
                // **********************************************
                // determinar o local de armazenagem da imagem
                string localizacaoImagem = _webHostEnvironment.WebRootPath;
                localizacaoImagem = Path.Combine(localizacaoImagem, "imagens");
                if (!Directory.Exists(localizacaoImagem)) {
                    Directory.CreateDirectory(localizacaoImagem);
                }
                // gerar o caminho completo para a imagem
                nomeImagem = Path.Combine(localizacaoImagem, nomeImagem);
                // agora, temos condições para guardar a imagem
                using var stream = new FileStream(
                    nomeImagem, FileMode.Create
                );
                await imagemFoto.CopyToAsync(stream);
                // **********************************************

                return RedirectToAction(nameof(Index));
            }
            
            // Se chego aqui é pq algo correu mal...
            return View(restaurantes);
        }

        // GET: Restaurantes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var restaurantes = await _context.Restaurantes.FindAsync(id);
            if (restaurantes == null)
            {
                return NotFound();
            }
            return View(restaurantes);
        }

        // POST: Restaurantes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
public async Task<IActionResult> Edit(int id, [Bind("Id,Nome,Endereco,CodPostal,HoraAbertura,HoraFecho,Foto")] Restaurantes restaurantes, IFormFile imagemFoto)
{
    if (id != restaurantes.Id)
    {
        return NotFound();
    }
    
    // Carregar a entidade original para manter Foto se necessário
    var restauranteOriginal = await _context.Restaurantes.AsNoTracking().FirstOrDefaultAsync(r => r.Id == id);
    if (restauranteOriginal == null)
    {
        return NotFound();
    }
    
    bool existeNome = await _context.Restaurantes
        .AnyAsync(r => r.Nome == restaurantes.Nome && r.Id != restaurantes.Id);
    if (existeNome)
    {
        ModelState.AddModelError("Nome", "Já existe um restaurante com este nome.");
        return View(restaurantes);
    }

    // Se não foi enviada nova imagem, manter a antiga
    if (imagemFoto == null)
    {
        restaurantes.Foto = restauranteOriginal.Foto;
    }
    else
    {
        // Validar o ficheiro da imagem
        if (imagemFoto.ContentType != "image/jpeg" && imagemFoto.ContentType != "image/png")
        {
            ModelState.AddModelError("", "A fotografia deve ser jpeg ou png.");
            return View(restaurantes);
        }

        // Processar a imagem nova
        string nomeImagem = Guid.NewGuid().ToString();
        string extensao = Path.GetExtension(imagemFoto.FileName).ToLowerInvariant();
        nomeImagem += extensao;

        restaurantes.Foto = nomeImagem;

        // Guardar o ficheiro no disco
        string localizacaoImagem = Path.Combine(_webHostEnvironment.WebRootPath, "imagens");
        if (!Directory.Exists(localizacaoImagem))
        {
            Directory.CreateDirectory(localizacaoImagem);
        }

        string caminhoCompleto = Path.Combine(localizacaoImagem, nomeImagem);
        using var stream = new FileStream(caminhoCompleto, FileMode.Create);
        await imagemFoto.CopyToAsync(stream);
    }

    if (ModelState.IsValid)
    {
        try
        {
            _context.Update(restaurantes);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!RestaurantesExists(restaurantes.Id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }
    }

    return View(restaurantes);
}


        // GET: Restaurantes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var restaurantes = await _context.Restaurantes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (restaurantes == null)
            {
                return NotFound();
            }

            return View(restaurantes);
        }

        // POST: Restaurantes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var restaurantes = await _context.Restaurantes.FindAsync(id);
            if (restaurantes != null)
            {
                _context.Restaurantes.Remove(restaurantes);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RestaurantesExists(int id)
        {
            return _context.Restaurantes.Any(e => e.Id == id);
        }
    }
}
