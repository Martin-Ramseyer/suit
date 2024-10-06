using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using suitMvc.Models;

namespace suitMvc.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }


[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        var requestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
        _logger.LogError($"Se produjo un error. Request ID: {requestId}");
        
        return View(new ErrorViewModel { RequestId = requestId });
    }
    }
}
