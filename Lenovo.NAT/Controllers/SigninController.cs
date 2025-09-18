using Lenovo.NAT.Infrastructure.Entities;
using Lenovo.NAT.Services.Admin;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Lenovo.NAT.Controllers
{
    public class SigninController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public SigninController(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }
        
        public IActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> Login()
        {
            return RedirectToAction("index", "home");
        }
    }
}
