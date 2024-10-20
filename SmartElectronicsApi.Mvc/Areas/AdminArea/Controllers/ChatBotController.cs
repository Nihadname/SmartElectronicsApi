using Microsoft.AspNetCore.Mvc;

namespace SmartElectronicsApi.Mvc.Areas.AdminArea.Controllers
{
    [Area("AdminArea")]
    public class ChatBotController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
