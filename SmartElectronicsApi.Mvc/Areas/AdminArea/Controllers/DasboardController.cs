using Microsoft.AspNetCore.Mvc;

namespace SmartElectronicsApi.Mvc.Areas.AdminArea.Controllers
{
    [Area("AdminArea")]
    public class DasboardController : Controller
    {
        
        public IActionResult Index()
        {
            return View();
        }
    }
}
