using Microsoft.AspNetCore.Mvc;

namespace CropsPriceTracker.UI.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Login()
        {
            return View();
        }
        public IActionResult Dashboard()
        {
            return View();
        }

        public IActionResult Analyze()
        {
            return View();
        }

        public IActionResult History()
        {
            return View();
        }

        public IActionResult AddAlert()
        {
            return View();
        }

        public IActionResult Alerts()
        {
            return View();
        }
    }
}
