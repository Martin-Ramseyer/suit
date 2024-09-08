using Microsoft.AspNetCore.Mvc;
using suitMvc.Data;

namespace suitMvc.Controllers
{
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
    }
}