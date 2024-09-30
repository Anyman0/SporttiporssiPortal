using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SporttiporssiPortal.Configurations;
using System.Diagnostics;
using SporttiporssiPortal.Models;
using SporttiporssiPortal.Interfaces;
using System.Text;
using System.Net.Http.Headers;

namespace SporttiporssiPortal.Controllers
{
    public class GamesController : BaseController
    {
        private readonly string _apiBaseUrl;
        private readonly HttpClient _httpClient;
       
        public GamesController(IOptions<ApiSettings> apiSettings, HttpClient httpClient, ISeriesService seriesService) : base(seriesService)
        {
            _apiBaseUrl = apiSettings.Value.DevBaseUrl;
            //_httpClient = httpClient;
            var unsafeHttpClient = new UnsafeHttpClientHandler();
            _httpClient = new HttpClient(unsafeHttpClient);
        }
        public async Task<IActionResult> Index()
        {
            var authToken = HttpContext.Session.GetString("auth_token");
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

            // Retrieve selected date from the session, default to today if not set
            var contextDate = HttpContext.Session.GetString("SelectedDate");
            DateTime date;

            if (string.IsNullOrEmpty(contextDate))
            {
                date = DateTime.UtcNow.Date;
                HttpContext.Session.SetString("SelectedDate", date.ToString("yyyy-MM-dd"));
            }
            else
            {
                date = DateTime.Parse(contextDate);
            }

            var selectedSeries = HttpContext.Session.GetString("SelectedSeries");           
            if (selectedSeries == null)
            {
                selectedSeries = "LIIGA";
                HttpContext.Session.SetString("SelectedSeries", selectedSeries);
            }            
            var formattedDate = date.ToString("MM.dd.yyyy");
            try
            {
                Debug.WriteLine($"Making api request to {_apiBaseUrl}Games?date={formattedDate}&serie={selectedSeries}");
                var response = await _httpClient.GetAsync($"{_apiBaseUrl}Games?date={formattedDate}&serie={selectedSeries}");
                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync();
                var games = JsonConvert.DeserializeObject<List<Game>>(json);

                ViewBag.Date = date.ToString("yyyy-MM-dd");
                return View(games);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error fetching games: {ex.Message}");
                return BadRequest(ex.Message);
            }
        }

        public IActionResult ChangeDate(DateTime date)
        {
            if (date == DateTime.MinValue)
            {
                date = DateTime.UtcNow.Date;
            }

            // Store the selected date in the session
            HttpContext.Session.SetString("SelectedDate", date.ToString("yyyy-MM-dd"));

            // Redirect to Index to load games for the selected date
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateGame(int id, int season)
        {
            var authToken = HttpContext.Session.GetString("auth_token");
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
            var selectedSeries = HttpContext.Session.GetString("SelectedSeries");
            // IN case of Liiga, we can update player stats aswell
            if(!string.IsNullOrEmpty(selectedSeries) && string.Equals(selectedSeries.ToLower(), "liiga"))
            {
                try
                {
                    var content = new StringContent("", Encoding.UTF8, "application/json");
                    Debug.WriteLine($"Making API request to: {_apiBaseUrl}Games/UpdateLiigaGameStats?season={season}&gameId={id}");
                    var response = await _httpClient.PostAsync($"{_apiBaseUrl}Games/UpdateLiigaGameStats?season={season}&gameId={id}", content);
                    response.EnsureSuccessStatusCode();

                    // Logic for after update
                    TempData["Message"] = "Game updated successfully!";
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error updating game: {ex.Message}");
                    TempData["ErrorMessage"] = "Failed to update game.";
                }
            }
            if(string.IsNullOrEmpty(HttpContext.Session.GetString("SelectedDate")))
            {
                ViewBag.Date = DateTime.UtcNow.Date;
            }
            else
            {
                ViewBag.Date = HttpContext.Session.GetString("SelectedDate");
            }
            return RedirectToAction("Index", new { date = ViewBag.Date });
        }

    }
}
