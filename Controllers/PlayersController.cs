using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;
using Newtonsoft.Json;
using SporttiporssiPortal.Configurations;
using SporttiporssiPortal.Interfaces;
using SporttiporssiPortal.Models;

namespace SporttiporssiPortal.Controllers
{
    public class PlayersController : BaseController
    {
        private readonly string _apiBaseUrl;
        private readonly HttpClient _httpClient;
        private ISeriesService _seriesService;
        public PlayersController(IOptions<ApiSettings> apiSettings, HttpClient httpClient, ISeriesService seriesService) : base(seriesService)
        {
            _apiBaseUrl = apiSettings.Value.DevBaseUrl;
            _seriesService = seriesService;
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
                ViewBag.NationalitySortParam = sortOrder == "Nationality" ? "Nationality_desc" : "Nationality";
                ViewBag.TournamentSortParam = sortOrder == "Tournament" ? "Tournament_desc" : "Tournament";
                ViewBag.PictureUrlSortParam = sortOrder == "PictureUrl" ? "PictureUrl_desc" : "PictureUrl";
                ViewBag.InjuredSortParam = sortOrder == "Injured" ? "Injured_desc" : "Injured";
                ViewBag.JerseySortParam = sortOrder == "Jersey" ? "Jersey_desc" : "Jersey";
                ViewBag.GoalkeeperSortParam = sortOrder == "Goalkeeper" ? "Goalkeeper_desc" : "Goalkeeper";               
                ViewBag.PlayedGamesSortParam = sortOrder == "PlayedGames" ? "PlayedGames_desc" : "PlayedGames";
                ViewBag.SuspendedSortParam = sortOrder == "Suspended" ? "Suspended_desc" : "Suspended";
                ViewBag.TimeOnIceSortParam = sortOrder == "TimeOnIce" ? "TimeOnIce_desc" : "TimeOnIce";
                ViewBag.GoalsSortParam = sortOrder == "Goals" ? "Goals_desc" : "Goals";
                ViewBag.AssistsSortParam = sortOrder == "Assists" ? "Assists_desc" : "Assists";
                ViewBag.PointsSortParam = sortOrder == "Points" ? "Points_desc" : "Points";
                ViewBag.PlusSortParam = sortOrder == "Plus" ? "Plus_desc" : "Plus";
                ViewBag.MinusSortParam = sortOrder == "Minus" ? "Minus_desc" : "Minus";
                ViewBag.PlusMinusSortParam = sortOrder == "PlusMinus" ? "PlusMinus_desc" : "PlusMinus";
                ViewBag.PenaltyMinutesSortParam = sortOrder == "PenaltyMinutes" ? "PenaltyMinutes_desc" : "PenaltyMinutes";
                ViewBag.Penalty2SortParam = sortOrder == "Penalty2" ? "Penalty2_desc" : "Penalty2";
                ViewBag.Penalty10SortParam = sortOrder == "Penalty10" ? "Penalty10_desc" : "Penalty10";
                ViewBag.Penalty20SortParam = sortOrder == "Penalty20" ? "Penalty20_desc" : "Penalty20";
                ViewBag.WinningGoalsSortParam = sortOrder == "WinningGoals" ? "WinningGoals_desc" : "WinningGoals";
                ViewBag.ShotsSortParam = sortOrder == "Shots" ? "Shots_desc" : "Shots";
                ViewBag.SavesSortParam = sortOrder == "Saves" ? "Saves_desc" : "Saves";
                ViewBag.GoalieShutoutSortParam = sortOrder == "GoalieShutout" ? "GoalieShutout_desc" : "GoalieShutout";
                ViewBag.AllowedGoalsSortParam = sortOrder == "AllowedGoals" ? "AllowedGoals_desc" : "AllowedGoals";
                ViewBag.FaceoffsWonSortParam = sortOrder == "FaceoffsWon" ? "FaceoffsWon_desc" : "FaceoffsWon";
                ViewBag.FaceoffsLostSortParam = sortOrder == "FaceoffsLost" ? "FaceoffsLost_desc" : "FaceoffsLost";
                ViewBag.TimeOnIceAvgSortParam = sortOrder == "TimeOnIceAvg" ? "TimeOnIceAvg_desc" : "TimeOnIceAvg";
                ViewBag.FaceoffWonPercentageSortParam = sortOrder == "FaceoffWonPercentage" ? "FaceoffWonPercentage_desc" : "FaceoffWonPercentage";
                ViewBag.ShotPercentageSortParam = sortOrder == "ShotPercentage" ? "ShotPercentage_desc" : "ShotPercentage";
                ViewBag.FaceoffsTotalSortParam = sortOrder == "FaceoffsTotal" ? "FaceoffsTotal_desc" : "FaceoffsTotal";
                ViewBag.LastUpdatedSortParam = sortOrder == "LastUpdated" ? "LastUpdated_desc" : "LastUpdated";
                ViewBag.FTPSortParam = sortOrder == "FTP" ? "FTP_desc" : "FTP";
                ViewBag.PriceSortParam = sortOrder == "Price" ? "Price_desc" : "Price";
                ViewBag.PlayerOwnedSortParam = sortOrder == "PlayerOwned" ? "PlayerOwned_desc" : "PlayerOwned";
                ViewBag.BlockedShotsSortParam = sortOrder == "BlockedShots" ? "BlockedShots_desc" : "BlockedShots";
                ViewBag.GameWonSortParam = sortOrder == "GameWon" ? "GameWon_desc" : "GameWon";

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
                    "Nationality" => players.OrderBy(p => p.Nationality).ToList(),
                    "Nationality_desc" => players.OrderByDescending(p => p.Nationality).ToList(),
                    "Tournament" => players.OrderBy(p => p.Tournament).ToList(),
                    "Tournament_desc" => players.OrderByDescending(p => p.Tournament).ToList(),
                    "PictureUrlSortParam" => players.OrderBy(p => p.PictureUrl).ToList(),
                    "PictureUrlSortParam_desc" => players.OrderByDescending(p =>p.PictureUrl).ToList(),
                    "Injured" => players.OrderBy(p => p.Injured).ToList(),
                    "Injured_desc" => players.OrderByDescending(p => p.Injured).ToList(),
                    "Jersey" => players.OrderBy(p => p.Jersey).ToList(),
                    "Jersey_desc" => players.OrderByDescending(p => p.Jersey).ToList(),
                    "Goalkeeper" => players.OrderBy(p => p.Goalkeeper).ToList(),
                    "Goalkeeper_desc" => players.OrderByDescending(p => p.Goalkeeper).ToList(),                    
                    "PlayedGames" => players.OrderBy(p => p.PlayedGames).ToList(),
                    "PlayedGames_desc" => players.OrderByDescending(p => p.PlayedGames).ToList(),
                    "Suspended" => players.OrderBy(p => p.Suspended).ToList(),
                    "Suspended_desc" => players.OrderByDescending(p => p.Suspended).ToList(),
                    "TimeOnIce" => players.OrderBy(p => p.TimeOnIce).ToList(),
                    "TimeOnIce_desc" => players.OrderByDescending(p => p.TimeOnIce).ToList(),
                    "Goals" => players.OrderBy(p => p.Goals).ToList(),
                    "Goals_desc" => players.OrderByDescending(p => p.Goals).ToList(),
                    "Assists" => players.OrderBy(p => p.Assists).ToList(),
                    "Assists_desc" => players.OrderByDescending(p => p.Assists).ToList(),
                    "Points" => players.OrderBy(p => p.Points).ToList(),
                    "Points_desc" => players.OrderByDescending(p => p.Points).ToList(),
                    "Plus" => players.OrderBy(p => p.Plus).ToList(),
                    "Plus_desc" => players.OrderByDescending(p => p.Plus).ToList(),
                    "Minus" => players.OrderBy(p => p.Minus).ToList(),
                    "Minus_desc" => players.OrderByDescending(p => p.Minus).ToList(),
                    "PlusMinus" => players.OrderBy(p => p.PlusMinus).ToList(),
                    "PlusMinus_desc" => players.OrderByDescending(p => p.PlusMinus).ToList(),
                    "PenaltyMinutes" => players.OrderBy(p => p.PenaltyMinutes).ToList(),
                    "PenaltyMinutes_desc" => players.OrderByDescending(p => p.PenaltyMinutes).ToList(),
                    "Penalty2" => players.OrderBy(p => p.Penalty2).ToList(),
                    "Penalty2_desc" => players.OrderByDescending(p => p.Penalty2).ToList(),
                    "Penalty10" => players.OrderBy(p => p.Penalty10).ToList(),
                    "Penalty10_desc" => players.OrderByDescending(p => p.Penalty10).ToList(),
                    "Penalty20" => players.OrderBy(p => p.Penalty20).ToList(),
                    "Penalty20_desc" => players.OrderByDescending(p => p.Penalty20).ToList(),
                    "WinningGoals" => players.OrderBy(p => p.WinningGoals).ToList(),
                    "WinningGoals_desc" => players.OrderByDescending(p => p.WinningGoals).ToList(),
                    "Shots" => players.OrderBy(p => p.Shots).ToList(),
                    "Shots_desc" => players.OrderByDescending(p => p.Shots).ToList(),
                    "Saves" => players.OrderBy(p => p.Saves).ToList(),
                    "Saves_desc" => players.OrderByDescending(p => p.Saves).ToList(),
                    "GoalieShutout" => players.OrderBy(p => p.GoalieShutout).ToList(),
                    "GoalieShutout_desc" => players.OrderByDescending(p => p.GoalieShutout).ToList(),
                    "AllowedGoals" => players.OrderBy(p => p.AllowedGoals).ToList(),
                    "AllowedGoals_desc" => players.OrderByDescending(p => p.AllowedGoals).ToList(),
                    "FaceoffsWon" => players.OrderBy(p => p.FaceoffsWon).ToList(),
                    "FaceoffsWon_desc" => players.OrderByDescending(p => p.FaceoffsWon).ToList(),
                    "FaceoffsLost" => players.OrderBy(p => p.FaceoffsLost).ToList(),
                    "FaceoffsLost_desc" => players.OrderByDescending(p => p.FaceoffsLost).ToList(),
                    "TimeOnIceAvg" => players.OrderBy(p => p.TimeOnIceAvg).ToList(),
                    "TimeOnIceAvg_desc" => players.OrderByDescending(p => p.TimeOnIceAvg).ToList(),
                    "FaceoffWonPercentage" => players.OrderBy(p => p.FaceoffWonPercentage).ToList(),
                    "FaceoffWonPercentage_desc" => players.OrderByDescending(p => p.FaceoffWonPercentage).ToList(),
                    "ShotPercentage" => players.OrderBy(p => p.ShotPercentage).ToList(),
                    "ShotPercentage_desc" => players.OrderByDescending(p => p.ShotPercentage).ToList(),
                    "FaceoffsTotal" => players.OrderBy(p => p.FaceoffsTotal).ToList(),
                    "FaceoffsTotal_desc" => players.OrderByDescending(p => p.FaceoffsTotal).ToList(),
                    "LastUpdated" => players.OrderBy(p => p.LastUpdated).ToList(),
                    "LastUpdated_desc" => players.OrderByDescending(p => p.LastUpdated).ToList(),
                    "FTP" => players.OrderBy(p => p.FTP).ToList(),
                    "FTP_desc" => players.OrderByDescending(p => p.FTP).ToList(),
                    "Price" => players.OrderBy(p => p.Price).ToList(),
                    "Price_desc" => players.OrderByDescending(p => p.Price).ToList(),
                    "PlayerOwned" => players.OrderBy(p => p.PlayerOwned).ToList(),
                    "PlayerOwned_desc" => players.OrderByDescending(p => p.PlayerOwned).ToList(),
                    "BlockedShots" => players.OrderBy(p => p.BlockedShots).ToList(),
                    "BlockedShots_desc" => players.OrderByDescending(p => p.BlockedShots).ToList(),
                    "GameWon" => players.OrderBy(p => p.GameWon).ToList(),
                    "GameWon_desc" => players.OrderByDescending(p => p.GameWon).ToList(),
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

        // GET: Show form to create a new player
        public IActionResult Create()
        {
            return View();
        }

        // POST: Handle the creation of a new player
        [HttpPost]
        public async Task<IActionResult> Create(Player player)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var authToken = HttpContext.Session.GetString("auth_token");
                    _httpClient.DefaultRequestHeaders.Clear();
                    _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
                    var selectedSeries = HttpContext.Session.GetString("SelectedSeries");
                    if(selectedSeries == null)
                    {
                        return BadRequest("Series not chosen");
                    }                  
                    var json = JsonConvert.SerializeObject(player);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    var response = await _httpClient.PostAsync($"{_apiBaseUrl}/player/postplayer?series={selectedSeries}", content);

                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction(nameof(Index));
                    }

                    ModelState.AddModelError(string.Empty, "Server error. Please try again.");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error creating player: {ex.Message}");
                    ModelState.AddModelError(string.Empty, "Server error. Please try again.");
                }
                return View(player);
            }
            else
            {
                return View(player);
            }
        }

        // GET: Show form to edit existing player
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var authToken = HttpContext.Session.GetString("auth_token");
                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
                Debug.WriteLine($"Making API call to: {_apiBaseUrl}Player/GetPlayerById?Id={id}");
                // Fetch player data from the API using the player's ID
                var response = await _httpClient.GetAsync($"{_apiBaseUrl}Player/GetPlayerById?Id={id}");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var player = JsonConvert.DeserializeObject<Player>(json);

                    // Pass the player object to the view
                    return View(player);
                }

                ModelState.AddModelError(string.Empty, "Unable to retrieve player data.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error fetching player: {ex.Message}");
                ModelState.AddModelError(string.Empty, "Server error. Please try again.");
            }

            return RedirectToAction(nameof(Index));
        }

        // PUT: Handle the editing of an existing player
        [HttpPost]
        public async Task<IActionResult> Edit(Player player)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var authToken = HttpContext.Session.GetString("auth_token");
                    _httpClient.DefaultRequestHeaders.Clear();
                    _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
                    var json = JsonConvert.SerializeObject(player);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    var response = await _httpClient.PutAsync($"{_apiBaseUrl}Player/UpdatePlayer", content);

                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction(nameof(Index));
                    }                   
                    ModelState.AddModelError(string.Empty, "Server error. Please try again.");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error updating player: {ex.Message}");
                    ModelState.AddModelError(string.Empty, "Server error. Please try again.");                   
                }
                return View(player);
            }
            else
            {
                return View(player);
            }
        }

        // GET: Show delete confirmation for selected player
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            try
            {
                var authToken = HttpContext.Session.GetString("auth_token");
                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
                Debug.WriteLine($"Making API call to: {_apiBaseUrl}Player/GetPlayerById?Id={id}");
                // Fetch player data from the API using the player's ID
                var response = await _httpClient.GetAsync($"{_apiBaseUrl}Player/GetPlayerById?Id={id}");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var player = JsonConvert.DeserializeObject<Player>(json);

                    // Pass the player object to the view
                    return View(player);
                }

                ModelState.AddModelError(string.Empty, "Unable to retrieve player data.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error fetching player: {ex.Message}");
                ModelState.AddModelError(string.Empty, "Server error. Please try again.");
            }

            return RedirectToAction(nameof(Index));
        }

        // Handle deletion of existing player
        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var authToken = HttpContext.Session.GetString("auth_token");
                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
                Debug.WriteLine($"Making API call to: {_apiBaseUrl}Player/Delete?id={id}");
                // Fetch player data from the API using the player's ID
                var response = await _httpClient.DeleteAsync($"{_apiBaseUrl}Player/Delete?id={id}");

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }

                ModelState.AddModelError(string.Empty, "Unable to retrieve player data.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error fetching player: {ex.Message}");
                ModelState.AddModelError(string.Empty, "Server error. Please try again.");
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
