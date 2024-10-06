using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using suitMvc.Data;
using suitMvc.Models;
using suitMvc.ViewModels;

namespace suitMvc.Controllers
{
    public class LoginController : Controller
    {
        private readonly SuitDbContext _context;
        public LoginController(SuitDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginVM modelo)
        {
            if (!ModelState.IsValid)
            {
                return View("Index", modelo);
            }

            Usuarios? usuario_encontrado = await _context.Usuarios
                .Where(u => u.usuario == modelo.usuario && u.contrasena == modelo.contrasena)
                .FirstOrDefaultAsync();

            if (usuario_encontrado == null || usuario_encontrado.usuario == null)
            {
                ViewData["Mensaje"] = "Usuario o contraseña incorrecta.";
                return View("Index", modelo);
            }

            List<Claim> claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, usuario_encontrado.usuario),
                new Claim(ClaimTypes.NameIdentifier, usuario_encontrado.usuario_id.ToString()), // Aquí se almacena la ID del usuario
                new Claim("admin", usuario_encontrado.admin.ToString()), // Aquí se almacena si el usuario es admin
                new Claim("cajero", usuario_encontrado.cajero.ToString()) // Aquí se almacena si el usuario es cajero
            };

            ClaimsIdentity identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            AuthenticationProperties properties = new AuthenticationProperties()
            {
                AllowRefresh = true,
            };
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity),
                properties
            );

            if (usuario_encontrado.admin == 1)
            {
                return RedirectToAction("Index", "Usuarios");
            }
            else if (usuario_encontrado.cajero == 1)
            {
                return RedirectToAction("ListarInvitados", "Invitados");
            }
            else
            {
                return RedirectToAction("Index", "Invitados");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
    }
}