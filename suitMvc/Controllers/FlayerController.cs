using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using suitMvc.Data;
using suitMvc.Models;
using System.Security.Claims;

namespace suitMvc.Controllers
{
    [Authorize]
    public class FlayerController : Controller
    {
        private readonly SuitDbContext _context;

        public FlayerController(SuitDbContext context)
        {
            _context = context;
        }

        // GET: Flayer
        public IActionResult Index()
        {
            return View();
        }

        // POST: Flayer/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Subir(IFormFile file, int usuario_id)
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

            if (file != null)
            {
                // Guardar la imagen en una carpeta local
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
                string filePath = Path.Combine(uploadsFolder, "flayer.jpg");

                using (FileStream stream = new(filePath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }

                // Actualizar la ruta de la imagen en la base de datos
                Flayer? flayer = _context.Flayers.FirstOrDefault(f => f.usuario_id == usuario_id);
                if (flayer != null)
                {
                    flayer.imagen = "/uploads/flayer.jpg";
                    _context.SaveChanges();
                }
            }

            return RedirectToAction(nameof(Index));
        }
    }
}