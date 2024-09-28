using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;
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
                .Where(i => userId != null && i.usuario_id == int.Parse(userId))
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
        [HttpGet]
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
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                var usuario = await _context.Usuarios.FindAsync(int.Parse(userId));
                if (usuario == null)
                {
                    return Unauthorized();
                }

                var invitado = await _context.Invitados.FindAsync(modelo.invitado_id);
                if (invitado == null)
                {
                    return NotFound();
                }

                // Actualiza solo las propiedades permitidas para usuarios no administradores
                invitado.nombre = modelo.nombre;
                invitado.apellido = modelo.apellido;
                invitado.acompanantes = modelo.acompanantes;

                _context.Update(invitado);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Beneficios(int id)
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
        public async Task<IActionResult> Beneficios(Invitados modelo)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                var usuario = await _context.Usuarios.FindAsync(int.Parse(userId));
                if (usuario == null)
                {
                    return Unauthorized();
                }

                var invitado = await _context.Invitados.FindAsync(modelo.invitado_id);
                if (invitado == null)
                {
                    return NotFound();
                }

                if (usuario.admin == 1)
                {
                    invitado.consumiciones = modelo.consumiciones;
                    invitado.entrada_free = modelo.entrada_free;
                    invitado.pulsera = modelo.pulsera;
                }

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
                .OrderBy(i => i.Usuarios.nombre)
                .ToListAsync();

            return View(invitados);
        }

        public async Task<IActionResult> ListaData()
        {
            var invitados = await _context.Invitados
                .Include(i => i.Usuarios)
                .ToListAsync();

            var cantidadInvitados = invitados.Count;
            var cantidadIngresados = invitados.Count(i => i.paso == 1);
            var cantidadPorUsuario = invitados.GroupBy(i => new { i.usuario_id, i.Usuarios.nombre, i.Usuarios.apellido })
                                              .Select(g => new
                                              {
                                                  g.Key.usuario_id,
                                                  g.Key.nombre,
                                                  g.Key.apellido,
                                                  Cantidad = g.Count(),
                                                  Ingresados = g.Count(i => i.paso == 1),
                                                  NoIngresados = g.Count(i => i.paso == 0)
                                              })
                                              .ToList();
            var cantidadEntradasFree = invitados.Sum(i => i.entrada_free);
            var cantidadConsumiciones = invitados.Sum(i => i.consumiciones);
            var cantidadPulseras = invitados.Sum(i => i.pulsera);

            ViewBag.CantidadInvitados = cantidadInvitados;
            ViewBag.CantidadIngresados = cantidadIngresados;
            ViewBag.CantidadPorUsuario = cantidadPorUsuario;
            ViewBag.CantidadEntradasFree = cantidadEntradasFree;
            ViewBag.CantidadConsumiciones = cantidadConsumiciones;
            ViewBag.CantidadPulseras = cantidadPulseras;

            return View();
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> InvitadoPaso(int id, int paso)
        {
            var invitado = await _context.Invitados.FindAsync(id);
            if (invitado == null)
            {
                return NotFound();
            }

            invitado.paso = (paso == 1) ? 0 : 1;

            _context.Update(invitado);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(ListarInvitados));
        }


        public async Task<IActionResult> ExportarExcel()
        {
            var invitados = await _context.Invitados
                .Include(i => i.Usuarios)
                .OrderBy(i => i.Usuarios.nombre)
                .ToListAsync();

            var cantidadInvitados = invitados.Count;
            var cantidadIngresados = invitados.Count(i => i.paso == 1);
            var cantidadPorUsuario = invitados.GroupBy(i => new { i.usuario_id, i.Usuarios.nombre, i.Usuarios.apellido })
                                              .Select(g => new
                                              {
                                                  g.Key.usuario_id,
                                                  g.Key.nombre,
                                                  g.Key.apellido,
                                                  Cantidad = g.Count(),
                                                  Ingresados = g.Count(i => i.paso == 1),
                                                  NoIngresados = g.Count(i => i.paso == 0)
                                              })
                                              .ToList();
            var cantidadEntradasFree = invitados.Sum(i => i.entrada_free);
            var cantidadConsumiciones = invitados.Sum(i => i.consumiciones);
            var cantidadPulseras = invitados.Sum(i => i.pulsera);

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Invitados");

                // Primera tabla: Lista de invitados
                worksheet.Cells["A1"].Value = "Nombre";
                worksheet.Cells["B1"].Value = "Apellido";
                worksheet.Cells["C1"].Value = "Acompañantes";
                worksheet.Cells["D1"].Value = "Entrada Free";
                worksheet.Cells["E1"].Value = "Consumiciones";
                worksheet.Cells["F1"].Value = "Pulsera";
                worksheet.Cells["G1"].Value = "Pública";
                worksheet.Cells["H1"].Value = "Ingreso";

                for (int i = 0; i < invitados.Count; i++)
                {
                    var invitado = invitados[i];
                    worksheet.Cells[i + 2, 1].Value = invitado.nombre;
                    worksheet.Cells[i + 2, 2].Value = invitado.apellido;
                    worksheet.Cells[i + 2, 3].Value = invitado.acompanantes;
                    worksheet.Cells[i + 2, 4].Value = invitado.entrada_free;
                    worksheet.Cells[i + 2, 5].Value = invitado.consumiciones;
                    worksheet.Cells[i + 2, 6].Value = invitado.pulsera;
                    worksheet.Cells[i + 2, 7].Value = $"{invitado.Usuarios.nombre} {invitado.Usuarios.apellido}";
                    worksheet.Cells[i + 2, 8].Value = invitado.paso == 1 ? "Sí" : "No";
                }

                worksheet.Cells["A1:H1"].Style.Font.Bold = true;
                worksheet.Cells["A1:H1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells["A1:H1"].Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                worksheet.Cells.AutoFitColumns();

                // Segunda tabla: Lista de datos
                int startColumn = 10; // Comienza dos columnas después de la primera tabla
                worksheet.Cells[1, startColumn].Value = "Pública";
                worksheet.Cells[1, startColumn + 1].Value = "Invitados";
                worksheet.Cells[1, startColumn + 2].Value = "Ingresados";
                worksheet.Cells[1, startColumn + 3].Value = "Total Entradas Free: " + cantidadEntradasFree;
                worksheet.Cells[1, startColumn + 4].Value = "Total de Consumiciones: " + cantidadConsumiciones;
                worksheet.Cells[1, startColumn + 5].Value = "Total de Pulseras: " + cantidadPulseras;

                int row = 2;
                foreach (var item in cantidadPorUsuario)
                {
                    worksheet.Cells[row, startColumn].Value = $"{item.nombre} {item.apellido}";
                    worksheet.Cells[row, startColumn + 1].Value = item.Cantidad;
                    worksheet.Cells[row, startColumn + 2].Value = item.Ingresados;
                    row++;
                }

                worksheet.Cells[1, startColumn, 1, startColumn + 5].Style.Font.Bold = true;
                worksheet.Cells[1, startColumn, 1, startColumn + 5].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[1, startColumn, 1, startColumn + 5].Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                worksheet.Cells.AutoFitColumns();

                var stream = new MemoryStream();
                package.SaveAs(stream);
                stream.Position = 0;
                var fileName = $"Invitados_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
        }
    }
}