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
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var invitados = await _context.Invitados
                .Where(i => i.usuario_id == int.Parse(userId))
                .ToListAsync();

            return View(invitados);
        }

        public IActionResult Crear()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(Invitados modelo)
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
            {
                return Unauthorized();
            }

            modelo.usuario_id = int.Parse(userId);

            _context.Invitados.Add(modelo);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Invitados");
        }
        public async Task<IActionResult> Actualizar(int id)
        {
            var invitado = await _context.Invitados.FindAsync(id);

            if (invitado == null)
            {
                return NotFound();
            }

            return View(invitado);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Actualizar(Invitados modelo)
        {
            try
            {
                var invitado = await _context.Invitados.FindAsync(modelo.invitado_id);
                Console.Write(invitado);
                if (invitado == null)
                {
                    return NotFound();
                }

                // Actualiza solo las propiedades permitidas
                invitado.consumiciones = modelo.consumiciones;
                invitado.entrada_free = modelo.entrada_free;


                _context.Update(invitado);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
            return RedirectToAction(nameof(Index));

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Eliminar(int id)
        {
            var invitado = await _context.Invitados.FindAsync(id);
            if (invitado != null)
            {
                _context.Invitados.Remove(invitado);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> ListarInvitados()
        {
            var invitados = await _context.Invitados
            .Include(i => i.Usuarios)
            .ToListAsync();

            return View(invitados);
        }

        [HttpPost]
        public IActionResult EliminarSeleccionados(int[] selectedIds)
        {
            if (selectedIds != null && selectedIds.Length > 0)
            {
                foreach (var id in selectedIds)
                {
                    var invitado = _context.Invitados.Find(id);
                    if (invitado != null)
                    {
                        _context.Invitados.Remove(invitado);
                    }
                }
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(ListarInvitados));
        }

    }
}