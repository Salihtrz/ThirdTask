using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using ThirdTask.WebUI.Models.JwtModels;
using ThirdTask.WebUI.Models.LoginModels;

namespace ThirdTask.WebUI.Controllers
{
    public class LoginController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public LoginController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Index(CheckAppUserModel model)
        {
            var client = _httpClientFactory.CreateClient();
            var jsonData = JsonConvert.SerializeObject(model);
            StringContent content = new StringContent(jsonData,Encoding.UTF8,"application/json");
            var responseMessage = await client.PostAsync("http://127.0.0.1:6000/services/auth/login",content);
            if(responseMessage.IsSuccessStatusCode)
            {
                var json = await responseMessage.Content.ReadAsStringAsync();
                var tokenObj = JsonConvert.DeserializeObject<TokenResponseModel>(json);
                HttpContext.Session.SetString("JWToken", tokenObj.Token);
                return RedirectToAction("Index", "Product");
            }
            return View();
        }
    }
}
