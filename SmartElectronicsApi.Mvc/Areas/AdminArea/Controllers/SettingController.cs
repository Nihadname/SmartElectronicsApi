using Microsoft.AspNetCore.Mvc;

namespace SmartElectronicsApi.Mvc.Areas.AdminArea.Controllers
{
    public class SettingController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
