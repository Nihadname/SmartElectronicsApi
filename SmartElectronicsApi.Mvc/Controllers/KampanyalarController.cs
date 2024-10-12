using Microsoft.AspNetCore.Mvc;

namespace SmartElectronicsApi.Mvc.Controllers
{
    public class KampanyalarController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
