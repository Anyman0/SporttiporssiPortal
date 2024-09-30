using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SporttiporssiPortal.Configurations;
using SporttiporssiPortal.Interfaces;
using SporttiporssiPortal.Models;
using System.Diagnostics;

namespace SporttiporssiPortal.Controllers
{
    public class UserController : BaseController
    {
        private readonly string _apiBaseUrl;
        private readonly HttpClient _httpClient;
        private ISeriesService _seriesService;

        public UserController(IOptions<ApiSettings> apiSettings, HttpClient httpClient,  ISeriesService seriesService) : base(seriesService)
        {
            _apiBaseUrl = apiSettings.Value.DevBaseUrl;
            _seriesService = seriesService;
            //_httpClient = httpClient;
            var unsafeHttpClient = new UnsafeHttpClientHandler();
            _httpClient = new HttpClient(unsafeHttpClient);
        }
        
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(LoginViewModel model)
        {
            if(!ModelState.IsValid)
            {
                return View(model);
            }
            try
            {               
                var request = new { email = model.Username, password = model.Password };
                Debug.WriteLine($"Making API call to: {_apiBaseUrl}User/adminlogin", request);               
                var response = await _httpClient.PostAsJsonAsync($"{_apiBaseUrl}User/adminlogin", request);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
                    var token = result.Token;
                    // Store token for session
                    HttpContext.Session.SetString("auth_token", token);
                    // store email to display for user
                    HttpContext.Session.SetString("user_email", model.Username);
                    return RedirectToAction("Index", "Home");
                }
                else if(response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    ModelState.AddModelError("Login failed", "This user is not an admin.");
                    return View(model);
                }
                else
                {
                    ModelState.AddModelError("Login failed", "Invalid email or password");
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in login: {ex.Message}");
                ModelState.AddModelError(string.Empty, "Server error. Please try again.");
                return View(model);
            }
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }

    }
}
