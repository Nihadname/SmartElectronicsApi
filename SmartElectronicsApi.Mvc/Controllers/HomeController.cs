using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace SmartElectronicsApi.Mvc.Controllers
{
    public class HomeController : Controller
    {
    
        public IActionResult Index()
        {
            return View();
        }

       
    }
}
