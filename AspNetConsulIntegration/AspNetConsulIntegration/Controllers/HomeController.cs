using AspNetConsulIntegration.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace AspNetConsulIntegration.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration)
        {
            _logger = logger;

            var someValue = configuration.GetSection("appsettings/qa4/web").GetValue<int>("SomeValue");

            // This will throw exception if the "What" prop is no defined in the config json object
            //var c = configuration.GetSection("appsettings/qa4/web").GetValue<int>("What");

            var someWholeSection = configuration.GetSection("appsettings/qa4/web").Value;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}