using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using suitMvc.Data;
using suitMvc.Models;

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
        public IActionResult Create(IFormFile file, int usuario_id)
        {
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