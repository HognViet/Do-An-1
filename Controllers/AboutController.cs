using Microsoft.AspNetCore.Mvc;

namespace San_Pham_Do_An1.Controllers
{
    public class AboutController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
