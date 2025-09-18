using Lenovo.NAT.Models;
using Lenovo.NAT.Services.Admin;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Lenovo.NAT.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly PermissionService _permissionService;

        public HomeController(ILogger<HomeController> logger, PermissionService permissionService)
        {
            _logger = logger;
            _permissionService = permissionService;
        }

        public async Task<IActionResult> Index()
        {
           // Dicionário de permissões
            var permissions = new Dictionary<string, string>
            {
                
                ["📦Picking Request"] = "Logistic",
                ["📝ONL Ticket"] = "Logistic"
            };

            // Dicionário de rotas
            var routes = new Dictionary<string, string>
            {
                ["📦Picking Request"] = @Url.Action("Index", "Picking", new { area = "Logistic" })!,
                ["📝ONL Ticket"] = @Url.Action("Index", "OnlTicket", new { area = "Logistic" })!
            };

            var allowedItems = await _permissionService.GetAllowedItems("", permissions);
            var cards = _permissionService.BuildCards(allowedItems, permissions);

            // Atribui ao ViewBag para uso nas Views
            ViewBag.Permissions = permissions;
            ViewBag.Routes = routes;
            ViewBag.Cards = cards;

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}