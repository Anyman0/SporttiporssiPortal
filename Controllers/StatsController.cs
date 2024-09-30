using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SporttiporssiPortal.Configurations;
using SporttiporssiPortal.Interfaces;
using SporttiporssiPortal.Models;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text;

namespace SporttiporssiPortal.Controllers
{
    public class StatsController : BaseController
    {
        private readonly string _apiBaseUrl;
        private readonly HttpClient _httpClient;
        private ISeriesService _seriesService;
        public StatsController(IOptions<ApiSettings> apiSettings, HttpClient httpClient, ISeriesService seriesService) : base(seriesService)
        {
            _seriesService = seriesService;
            _apiBaseUrl = apiSettings.Value.DevBaseUrl;
            //_httpClient = httpClient;
            var unsafeHttpClient = new UnsafeHttpClientHandler();
            _httpClient = new HttpClient(unsafeHttpClient);
        }

        // GET: Players
        public async Task<IActionResult> Index(string searchString, string sortOrder, string currentFilter)
        {
            var authToken = HttpContext.Session.GetString("auth_token");
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
            var selectedSeries = HttpContext.Session.GetString("SelectedSeries");
            if (selectedSeries == null)
            {
                selectedSeries = "LIIGA";
                HttpContext.Session.SetString("SelectedSeries", selectedSeries);
            }
            ViewBag.CurrentFilter = searchString;
            try
            {
                var response = await _httpClient.GetAsync($"{_apiBaseUrl}Player?series={selectedSeries}");
                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync();
                var players = JsonConvert.DeserializeObject<List<Player>>(json);

                // Filter players by search string if provided
                if (!String.IsNullOrEmpty(searchString))
                {
                    var csString = searchString.ToLower();
                    players = players.Where(p => p.FirstName.ToLower().Contains(csString) || 
                                                 p.LastName.ToLower().Contains(csString) || 
                                                 p.TeamName.ToLower().Contains(csString) || 
                                                 p.TeamShortName.ToLower().Contains(csString)).ToList();
                }

                // Sorting logic
                ViewBag.IdSortParam = String.IsNullOrEmpty(sortOrder) ? "id_desc" : "";
                ViewBag.PlayerIdSortParam = sortOrder == "PlayerId" ? "PlayerId_desc" : "PlayerId";
                ViewBag.TeamIdSortParam = sortOrder == "TeamId" ? "TeamId_desc" : "TeamId";
                ViewBag.TeamNameSortParam = sortOrder == "TeamName" ? "TeamName_desc" : "TeamName";
                ViewBag.TeamShortNameSortParam = sortOrder == "TeamShortName" ? "TeamShortName_desc" : "TeamShortName";
                ViewBag.RoleSortParam = sortOrder == "Role" ? "Role_desc" : "Role";
                ViewBag.FirstNameSortParam = sortOrder == "FirstName" ? "FirstName_desc" : "FirstName";
                ViewBag.LastNameSortParam = sortOrder == "LastName" ? "LastName_desc" : "LastName";                
                ViewBag.JerseySortParam = sortOrder == "Jersey" ? "Jersey_desc" : "Jersey";
                ViewBag.LastUpdatedSortParam = sortOrder == "LastUpdated" ? "LastUpdated_desc" : "LastUpdated";

                players = sortOrder switch
                {
                    "id_desc" => players.OrderByDescending(p => p.Id).ToList(),
                    "PlayerId" => players.OrderBy(p => p.PlayerId).ToList(),
                    "PlayerId_desc" => players.OrderByDescending(p => p.PlayerId).ToList(),
                    "TeamId" => players.OrderBy(p => p.TeamId).ToList(),
                    "TeamId_desc" => players.OrderByDescending(p => p.TeamId).ToList(),
                    "TeamName" => players.OrderBy(p => p.TeamName).ToList(),
                    "TeamName_desc" => players.OrderByDescending(p => p.TeamName).ToList(),
                    "TeamShortName" => players.OrderBy(p => p.TeamShortName).ToList(),
                    "TeamShortName_desc" => players.OrderByDescending(p => p.TeamShortName).ToList(),
                    "Role" => players.OrderBy(p => p.Role).ToList(),
                    "Role_desc" => players.OrderByDescending(p => p.Role).ToList(),
                    "FirstName" => players.OrderBy(p => p.FirstName).ToList(),
                    "FirstName_desc" => players.OrderByDescending(p => p.FirstName).ToList(),
                    "LastName" => players.OrderBy(p => p.LastName).ToList(),
                    "LastName_desc" => players.OrderByDescending(p => p.LastName).ToList(),                    
                    "Jersey" => players.OrderBy(p => p.Jersey).ToList(),
                    "Jersey_desc" => players.OrderByDescending(p => p.Jersey).ToList(),
                    "LastUpdated" => players.OrderBy(p => p.LastUpdated).ToList(),
                    "LastUpdated_desc" => players.OrderByDescending(p => p.LastUpdated).ToList(),
                    _ => players.OrderBy(p => p.Id).ToList(),  // Default sorting by Id
                };
                return View(players);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error fetching players: {ex.Message}");
                return BadRequest(ex.Message);
            }
        }
        // PUT: Update player stats
        [HttpPost]
        public async Task<IActionResult> UpdatePlayer(UpdatePlayer player)
        {
            var authToken = HttpContext.Session.GetString("auth_token");
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
            var selectedSeries = HttpContext.Session.GetString("SelectedSeries");
            try
            {
                player.PenaltyMinutes = (player.Penalty2 * 2) + (player.Penalty10 * 10) + (player.Penalty20 * 20);
                player.FaceoffsTotal = player.FaceoffsLost + player.FaceoffsWon;
                // In case of Liiga, we update plusminus within the game and shouldnt update it here.
                if(!string.IsNullOrEmpty(selectedSeries) && selectedSeries.ToLower() == "liiga")
                {
                    player.PlusMinus = 0;
                }
                else
                {
                    player.PlusMinus = player.Plus - player.Minus;
                }                
                var json = JsonConvert.SerializeObject(player);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync($"{_apiBaseUrl}Player/UpdatePlayerStats", content);
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }
                ModelState.AddModelError(string.Empty, "Server error. Please try again.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error fetching player: {ex.Message}");
                ModelState.AddModelError(string.Empty, "Server error. Please try again.");
            }
            return View(player);
        }


        public IActionResult Update(UpdatePlayer player)
        {
            return View(player);
        }
    }
}
