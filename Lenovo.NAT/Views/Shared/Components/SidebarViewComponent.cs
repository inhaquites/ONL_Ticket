using Lenovo.NAT.Services.Admin;
using Microsoft.AspNetCore.Mvc;

namespace YourAppNamespace.Components
{
    public class SidebarViewComponent : ViewComponent
    {
        private readonly PermissionService _permissionService;

        public SidebarViewComponent(PermissionService permissionService)
        {
            _permissionService = permissionService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var domain = "LENOVO\\";
            var userNetworkId = string.Empty;

            var permissions = GetPermissions();
            var routes = GetRoutes();

            var allowedItems = await _permissionService.GetAllowedItems(userNetworkId, permissions);
            var cards = _permissionService.BuildCards(allowedItems, permissions);

            var model = new Lenovo.NAT.ViewModels.SidebarViewModel
            {
                Cards = cards,
                Routes = routes,
                Permissions = permissions
            };

            return View("Default", model);
        }

        private Dictionary<string, string> GetPermissions()
        {
            return new Dictionary<string, string>
            {
                ["📦Picking Request"] = "Logistic",
                ["📝ONL Ticket"] = "Logistic"
            };
        }

        private Dictionary<string, string> GetRoutes()
        {
            return new Dictionary<string, string>
            {
                ["📦Picking Request"] = @Url.Action("Index", "Picking", new { area = "Logistic" })!,
                ["📝ONL Ticket"] = @Url.Action("Index", "OnlTicket", new { area = "Logistic" })!
            };
        }
    }
}
