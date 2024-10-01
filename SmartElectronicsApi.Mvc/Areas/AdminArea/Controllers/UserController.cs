using Microsoft.AspNetCore.Mvc;

namespace SmartElectronicsApi.Mvc.Areas.AdminArea.Controllers
{
    public class UserController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
