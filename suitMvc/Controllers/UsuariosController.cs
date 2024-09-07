using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using suitMvc.Data;
using suitMvc.Models;

namespace suitMvc.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly SuitDbContext _context;
        public UsuariosController(SuitDbContext context)
        {
            _context = context;
        }

        //GET : Usuarios
        public async Task<IActionResult> Index()
        {
            return View(await _context.Usuarios.ToListAsync());
        }
        //GET : Usuarios/Crear
        public IActionResult Crear()
        {
            return View();
        }

        //POST : Usuario/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear([Bind("usuario_id,nombre,apellido,usuario,contrasena,admin")] Usuarios usuarios)
        {
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
            var usuario = _context.Usuarios.Find(id);
            if (usuario == null)
            {
                return NotFound();
            }
            return View(usuario);
        }

        //POST : Usuarios/Actualizar
        [HttpPost]
        public async Task<IActionResult> Actualizar(Usuarios usuarios)
        {
            if (ModelState.IsValid)
            {
                _context.Update(usuarios);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(usuarios);
        }

        //POST : Usuarios/Eliminar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Eliminar(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }

            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}


