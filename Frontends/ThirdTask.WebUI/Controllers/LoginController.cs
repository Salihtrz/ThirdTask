using Microsoft.AspNetCore.Mvc;

namespace ThirdTask.WebUI.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
