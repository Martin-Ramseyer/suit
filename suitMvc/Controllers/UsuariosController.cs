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

    }
}
