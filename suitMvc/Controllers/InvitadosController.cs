using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using suitMvc.Data;
using suitMvc.Models;

namespace suitMvc.Controllers
{
    [Authorize]
    public class InvitadosController : Controller
    {
        private readonly SuitDbContext _context;
        public InvitadosController(SuitDbContext context)
        {
            _context = context;
        }

        //GET : Invitados
        public async Task<IActionResult> Index()
        {
            return View(await _context.Invitados.Include(i => i.Usuarios).ToListAsync());
        }

        //GET : Invitados/Crear
        public IActionResult Crear()
        {
            return View();
        }

        //POST : Invitados/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(Invitados modelo)
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Obtiene la ID del usuario logeado

            if (userId == null)
            {
                return Unauthorized(); // Si el usuario no está logeado
            }

            modelo.usuario_id = int.Parse(userId); // Asocia la ID del usuario que lo creó

            _context.Invitados.Add(modelo);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Invitados");
        }
    }
}
