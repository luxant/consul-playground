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

        private static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }

        public IActionResult DnsClient()
        {
            //var dnsClient = new LookupClient(IPAddress.Parse(GetLocalIPAddress()), 8600);
            var dnsClient = new LookupClient(IPAddress.Parse("127.0.0.1"), 8600);

            var queryResult = dnsClient.Query("web.service.dc1.consul", QueryType.ANY);

            return View(queryResult);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}