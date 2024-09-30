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
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var invitado = await _context.Invitados.FindAsync(id);

            if (invitado == null || invitado.usuario_id != int.Parse(userId))
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
                if (invitado == null || invitado.usuario_id != int.Parse(userId))
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
                if (usuario == null || usuario.admin != 1)
                {
                    return Unauthorized();
                }

                var invitado = await _context.Invitados.FindAsync(modelo.invitado_id);
                if (invitado == null)
                {
                    return NotFound();
                }

                invitado.consumiciones = modelo.consumiciones;
                invitado.entrada_free = modelo.entrada_free;
                invitado.pulsera = modelo.pulsera;

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
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var usuario = await _context.Usuarios.FindAsync(int.Parse(userId));
            if (usuario == null || usuario.admin != 1)
            {
                return Unauthorized();
            }

            var invitado = await _context.Invitados.FindAsync(id);
            if (invitado == null)
            {
                return NotFound();
            }
            return View(invitado);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Eliminar(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var usuario = await _context.Usuarios.FindAsync(int.Parse(userId));
            if (usuario == null || usuario.admin != 1)
            {
                return Unauthorized();
            }

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
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = User.Claims.Any(c => c.Type == "admin" && c.Value == "1");
            var isCajero = User.Claims.Any(c => c.Type == "cajero" && c.Value == "1");

            if (!isAdmin && !isCajero)
            {
                return Unauthorized();
            }

            var invitados = await _context.Invitados
                .Include(i => i.Usuarios)
                .OrderBy(i => i.Usuarios.nombre)
                .ToListAsync();

            return View(invitados);
        }

        public async Task<IActionResult> ListaData()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = User.Claims.Any(c => c.Type == "admin" && c.Value == "1");
            var isCajero = User.Claims.Any(c => c.Type == "cajero" && c.Value == "1");

            if (!isAdmin && !isCajero)
            {
                return Unauthorized();
            }

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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> InvitadoPaso(int id, int paso)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = User.Claims.Any(c => c.Type == "admin" && c.Value == "1");
            var isCajero = User.Claims.Any(c => c.Type == "cajero" && c.Value == "1");

            if (!isAdmin && !isCajero)
            {
                return Unauthorized();
            }

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
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = User.Claims.Any(c => c.Type == "admin" && c.Value == "1");

            if (!isAdmin)
            {
                return Unauthorized();
            }

            // Obtener los datos necesarios de la base de datos
            var invitados = await _context.Invitados
                .Include(i => i.Usuarios)
                .OrderBy(i => i.Usuarios.nombre)
                .Select(i => new
                {
                    i.nombre,
                    i.apellido,
                    i.acompanantes,
                    i.entrada_free,
                    i.consumiciones,
                    i.pulsera,
                    i.paso,
                    UsuarioNombre = i.Usuarios.nombre,
                    UsuarioApellido = i.Usuarios.apellido
                })
                .ToListAsync();

            // Cálculos importantes
            var totalInvitados = invitados.Count;
            var totalIngresados = invitados.Count(i => i.paso == 1);
            var porcentajeIngresados = (totalIngresados / (double)totalInvitados) * 100;
            var totalEntradasFree = invitados.Sum(i => i.entrada_free);
            var totalConsumiciones = invitados.Sum(i => i.consumiciones);
            var totalPulseras = invitados.Sum(i => i.pulsera);
            var porcentajeEntradaFreeUsadas = (totalEntradasFree / (double)totalInvitados) * 100;
            var promedioConsumicionesPorInvitado = totalConsumiciones / (double)totalInvitados;

            // Agrupación por usuario
            var cantidadPorUsuario = invitados
                .GroupBy(i => new { i.UsuarioNombre, i.UsuarioApellido })
                .Select(g => new
                {
                    g.Key.UsuarioNombre,
                    g.Key.UsuarioApellido,
                    Cantidad = g.Count(),
                    Ingresados = g.Count(i => i.paso == 1),
                    NoIngresados = g.Count(i => i.paso == 0),
                    Consumiciones = g.Sum(i => i.consumiciones),
                    EntradasFree = g.Sum(i => i.entrada_free),
                    Pulseras = g.Sum(i => i.pulsera)
                })
                .ToList();

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Reporte Suit");

                // Estilos generales
                var headerStyle = worksheet.Cells["A1:H1"].Style;
                headerStyle.Font.Bold = true;
                headerStyle.Fill.PatternType = ExcelFillStyle.Solid;
                headerStyle.Fill.BackgroundColor.SetColor(Color.LightGray);
                headerStyle.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                // Estilo para los bordes
                var allCells = worksheet.Cells[1, 1, invitados.Count + 1, 8];
                allCells.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                allCells.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                allCells.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                allCells.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                // Primera tabla: Lista detallada de invitados
                var headers = new[] { "Nombre", "Apellido", "Acompañantes", "Entrada Free", "Consumiciones", "Pulsera", "Pública", "Ingreso" };
                for (int i = 0; i < headers.Length; i++)
                {
                    worksheet.Cells[1, i + 1].Value = headers[i];
                }

                for (int i = 0; i < invitados.Count; i++)
                {
                    var invitado = invitados[i];
                    worksheet.Cells[i + 2, 1].Value = invitado.nombre;
                    worksheet.Cells[i + 2, 2].Value = invitado.apellido;
                    worksheet.Cells[i + 2, 3].Value = invitado.acompanantes;
                    worksheet.Cells[i + 2, 4].Value = invitado.entrada_free;
                    worksheet.Cells[i + 2, 5].Value = invitado.consumiciones;
                    worksheet.Cells[i + 2, 6].Value = invitado.pulsera;
                    worksheet.Cells[i + 2, 7].Value = $"{invitado.UsuarioNombre} {invitado.UsuarioApellido}";
                    worksheet.Cells[i + 2, 8].Value = invitado.paso == 1 ? "Sí" : "No";
                }

                // Segunda tabla: Resumen de datos generales
                int startColumn = 10;
                worksheet.Cells[1, startColumn].Value = "Datos Generales";
                worksheet.Cells[2, startColumn].Value = "Total de Invitados";
                worksheet.Cells[2, startColumn + 1].Value = totalInvitados;
                worksheet.Cells[3, startColumn].Value = "Total invitados que ingresaron";
                worksheet.Cells[3, startColumn + 1].Value = totalIngresados;
                worksheet.Cells[4, startColumn].Value = "Porcentaje Ingresados";
                worksheet.Cells[4, startColumn + 1].Value = $"{porcentajeIngresados:N2}%";
                worksheet.Cells[5, startColumn].Value = "Total Entradas Free";
                worksheet.Cells[5, startColumn + 1].Value = totalEntradasFree;
                worksheet.Cells[6, startColumn].Value = "Porcentaje Entradas Free Usadas";
                worksheet.Cells[6, startColumn + 1].Value = $"{porcentajeEntradaFreeUsadas:N2}%";
                worksheet.Cells[7, startColumn].Value = "Total Consumiciones";
                worksheet.Cells[7, startColumn + 1].Value = totalConsumiciones;
                worksheet.Cells[8, startColumn].Value = "Promedio Consumiciones por Invitado";
                worksheet.Cells[8, startColumn + 1].Value = promedioConsumicionesPorInvitado;
                worksheet.Cells[9, startColumn].Value = "Total Pulseras";
                worksheet.Cells[9, startColumn + 1].Value = totalPulseras;

                // Aplicar estilos a la segunda tabla
                var generalDataCells = worksheet.Cells[1, startColumn, 9, startColumn + 1];
                generalDataCells.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                generalDataCells.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                generalDataCells.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                generalDataCells.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                generalDataCells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                generalDataCells.Style.Fill.BackgroundColor.SetColor(Color.LightYellow);
                generalDataCells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                generalDataCells.Style.Font.Bold = true;

                // Tercera tabla: Datos por usuario
                worksheet.Cells[11, startColumn].Value = "Datos de públicas";
                worksheet.Cells[12, startColumn].Value = "Pública";
                worksheet.Cells[12, startColumn + 1].Value = "Cantidad Invitados";
                worksheet.Cells[12, startColumn + 2].Value = "Ingresados";
                worksheet.Cells[12, startColumn + 3].Value = "No Ingresados";

                int row = 13;
                foreach (var item in cantidadPorUsuario)
                {
                    worksheet.Cells[row, startColumn].Value = $"{item.UsuarioNombre} {item.UsuarioApellido}";
                    worksheet.Cells[row, startColumn + 1].Value = item.Cantidad;
                    worksheet.Cells[row, startColumn + 2].Value = item.Ingresados;
                    worksheet.Cells[row, startColumn + 3].Value = item.NoIngresados;
                    row++;
                }

                // Aplicar estilos a la tercera tabla
                var userDataCells = worksheet.Cells[11, startColumn, row - 1, startColumn + 3];
                userDataCells.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                userDataCells.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                userDataCells.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                userDataCells.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                userDataCells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                userDataCells.Style.Fill.BackgroundColor.SetColor(Color.LightBlue);
                userDataCells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                userDataCells.Style.Font.Bold = true;

                worksheet.Cells.AutoFitColumns();

                // Exportar archivo Excel
                var stream = new MemoryStream();
                package.SaveAs(stream);
                stream.Position = 0;
                var fileName = $"Suit-{DateTime.Now:dd-MM-yyyy}.xlsx";
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
        }
    }
}