using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using suitMvc.Data;
using suitMvc.Models;

namespace suitMvc.Controllers
{
    [Authorize]
    public class UsuariosController : Controller
    {
        private readonly SuitDbContext _context;
        public UsuariosController(SuitDbContext context) => _context = context;

        //GET : Usuarios
        public async Task<IActionResult> Index()
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var usuario = await _context.Usuarios.FindAsync(int.Parse(userId));

            if (usuario == null || usuario.admin == 0)
            {
                return Unauthorized();
            }

            return View(await _context.Usuarios.ToListAsync());
        }

        //GET : Usuarios/Crear
        public IActionResult Crear()
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var usuario = _context.Usuarios.Find(int.Parse(userId));

            if (usuario == null || usuario.admin == 0)
            {
                return Unauthorized();
            }

            return View();
        }

        //POST : Usuario/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear([Bind("usuario_id,nombre,apellido,usuario,contrasena,admin")] Usuarios usuarios)
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var usuario = _context.Usuarios.Find(int.Parse(userId));

            if (usuario == null || usuario.admin == 0)
            {
                return Unauthorized();
            }

            if (ModelState.IsValid)
            {
                _context.Add(usuarios);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View();
        }

        //GET : Usuarios/Actualizar
        [HttpGet]
        public IActionResult Actualizar(int id)
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var usuario = _context.Usuarios.Find(int.Parse(userId));

            if (usuario == null || usuario.admin == 0)
            {
                return Unauthorized();
            }

            var usuarioToUpdate = _context.Usuarios.Find(id);
            if (usuarioToUpdate == null)
            {
                return NotFound();
            }
            return View(usuarioToUpdate);
        }

        //POST : Usuarios/Actualizar
        [HttpPost]
        public async Task<IActionResult> Actualizar(Usuarios usuarios)
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var usuario = _context.Usuarios.Find(int.Parse(userId));

            if (usuario == null || usuario.admin == 0)
            {
                return Unauthorized();
            }

            if (ModelState.IsValid)
            {
                _context.Update(usuarios);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(usuarios);
        }

        //POST : Usuarios/Eliminar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Eliminar(int id)
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var usuario = _context.Usuarios.Find(int.Parse(userId));

            if (usuario == null || usuario.admin == 0)
            {
                return Unauthorized();
            }

            var usuarioToDelete = await _context.Usuarios.FindAsync(id);
            if (usuarioToDelete == null)
            {
                return NotFound();
            }

            _context.Usuarios.Remove(usuarioToDelete);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
