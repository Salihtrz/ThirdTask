using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ThirdTask.WebUI.Models.ProductModels;

namespace ThirdTask.WebUI.Controllers
{
    public class ProductController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ProductController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var client = _httpClientFactory.CreateClient();
            var token = HttpContext.Session.GetString("JWToken");
            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
            var responseMessage = await client.GetAsync("http://127.0.0.1:6000/services/product");
            if(responseMessage.IsSuccessStatusCode)
            {
                var jsonData = await responseMessage.Content.ReadAsStringAsync();
                var values = JsonConvert.DeserializeObject<List<ResultProductModel>>(jsonData);
                return View(values);
            }
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var client = _httpClientFactory.CreateClient();
            var token = HttpContext.Session.GetString("JWToken");
            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
            var responseMessage = await client.GetAsync("http://127.0.0.1:6000/services/product/" + id);
            if (responseMessage.IsSuccessStatusCode)
            {
                var jsonData = await responseMessage.Content.ReadAsStringAsync();
                var value = JsonConvert.DeserializeObject<GetProductModel>(jsonData);
                return View(value);
            }
            return View();
        }
        [HttpGet]
        public IActionResult CreateProduct()
        {
            var client = _httpClientFactory.CreateClient();
            var token = HttpContext.Session.GetString("JWToken");
            string role = "";
            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);

                role = jwtToken.Claims
                .FirstOrDefault(c => c.Type == "role" ||
                c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role")?.Value;
            }
            if (role == "Writer")
                return View();
            if(role == "Reader")
                return RedirectToAction("Forbidden403", "Error");
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateProduct(CreateProductModel model)
        {
            model.Status = true;
            var client = _httpClientFactory.CreateClient();
            var token = HttpContext.Session.GetString("JWToken");
            string role = "";
            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);

                role = jwtToken.Claims
                .FirstOrDefault(c => c.Type == "role" ||
                c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role")?.Value;
            }
            var jsonData = JsonConvert.SerializeObject(model);
            StringContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");
            var responseMessage = await client.PostAsync("http://127.0.0.1:6000/services/product", content);
            if (responseMessage.IsSuccessStatusCode && role == "Writer")
            {
                return RedirectToAction("Index", "Product");
            }
            if (responseMessage.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                return RedirectToAction("Forbidden403", "Error");
            }
            if (responseMessage.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return RedirectToAction("PageNotFound404", "Error");
            }
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> UpdateProduct(int id)
        {
            var client = _httpClientFactory.CreateClient();
            var token = HttpContext.Session.GetString("JWToken");
            string role = "";
            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);

                role = jwtToken.Claims
                .FirstOrDefault(c => c.Type == "role" ||
                c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role")?.Value;
            }
            var responseMessage = await client.GetAsync("http://127.0.0.1:6000/services/product/" + id);
            if (responseMessage.IsSuccessStatusCode && role == "Writer")
            {
                var jsonData = await responseMessage.Content.ReadAsStringAsync();
                var value = JsonConvert.DeserializeObject<UpdateProductModel>(jsonData);
                return View(value);
            }
            if (role == "Reader")
            {
                return RedirectToAction("Forbidden403", "Error");
            }
            if (responseMessage.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return RedirectToAction("PageNotFound404", "Error");
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> UpdateProduct(UpdateProductModel model)
        {
            var client = _httpClientFactory.CreateClient();
            var token = HttpContext.Session.GetString("JWToken");
            string role = "";
            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);

                role = jwtToken.Claims
                .FirstOrDefault(c => c.Type == "role" ||
                c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role")?.Value;
            }
            var jsonData = JsonConvert.SerializeObject(model);
            StringContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");
            var responseMessage = await client.PutAsync("http://127.0.0.1:6000/services/product", content);
            if (responseMessage.IsSuccessStatusCode && role == "Writer")
            {
                return RedirectToAction("Index", "Product");
            }
            if(responseMessage.StatusCode  == System.Net.HttpStatusCode.Forbidden)
            {
                return RedirectToAction("Forbidden403","Error");
            }
            if (responseMessage.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return RedirectToAction("PageNotFound404", "Error");
            }
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var client = _httpClientFactory.CreateClient();
            var token = HttpContext.Session.GetString("JWToken");
            string role = "";
            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);

                role = jwtToken.Claims
                .FirstOrDefault(c => c.Type == "role" ||
                c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role")?.Value;
            }
            var responseMessage = await client.DeleteAsync("http://127.0.0.1:6000/services/product/" + id);
            if (responseMessage.IsSuccessStatusCode && role == "Writer")
            {
                return RedirectToAction("Index", "Product");
            }
            if (role == "Reader")
            {
                return RedirectToAction("Forbidden403", "Error");
            }
            if (responseMessage.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return RedirectToAction("PageNotFound404", "Error");
            }
            return View();
        }
    }
}
