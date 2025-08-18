using Microsoft.AspNetCore.Mvc;

namespace ThirdTask.WebUI.Controllers
{
    public class ErrorController : Controller
    {
        public IActionResult PageNotFound404()
        {
            return View();
        }
        public IActionResult Forbidden403()
        {
            return View();
        }
    }
}
