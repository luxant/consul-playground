using AspNetConsulIntegration.Models;
using Consul;
using DnsClient;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;

namespace AspNetConsulIntegration.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IAppConfig _appConfig;
        private readonly IConsulClient _consulClient;

        public HomeController(ILogger<HomeController> logger, IAppConfig appConfig, IConsulClient consulClient)
        {
            _logger = logger;
            _appConfig = appConfig;
            _consulClient = consulClient;
        }

        public IActionResult Index()
        {
            return View(_appConfig);
        }

        public IActionResult Info()
        {
            var addresses = Dns.GetHostAddresses(Dns.GetHostName());

            var result = addresses.Where(x => x.AddressFamily == AddressFamily.InterNetwork)
                .Select(x => new
                {
                    AddressFamily = Enum.GetName(x.AddressFamily),
                    Address = x.ToString(),
                });

            return Json(result, new System.Text.Json.JsonSerializerOptions
            {
                WriteIndented = true,
            });
        }

        public async Task<IActionResult> Services()
        {
            var services = _consulClient.Catalog.Services();
            var webServiceInfo = _consulClient.Catalog.Service("web");

            return View(new 
            {
                Services = await services,
                WebServiceInfo = await webServiceInfo
            });
        }

        public IActionResult DnsClient()
        {
            var dnsClient = new LookupClient(IPAddress.Parse("127.0.0.1"), 8600);

            var queryResult = dnsClient.Query("web.service.consul", QueryType.ANY);

            return View(queryResult);
        }


        public async Task<IActionResult> LoadBalance()
        {
            var dnsClient = new LookupClient(IPAddress.Parse("127.0.0.1"), 8600);

            var queryResult = dnsClient.Query("web.service.consul", QueryType.SRV);

            var address  = queryResult.AllRecords.AddressRecords().First();

            var httpClient = new HttpClient();
            var response = await httpClient.GetAsync($"http://{address.Address}:5000/Home/Info");

            return View(model: await response.Content.ReadAsStringAsync());
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}