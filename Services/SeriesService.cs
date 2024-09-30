using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SporttiporssiPortal.Configurations;
using SporttiporssiPortal.Interfaces;
using SporttiporssiPortal.Models;
using System.Diagnostics;

namespace SporttiporssiPortal.Services
{
    public class SeriesService : ISeriesService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl;

        public SeriesService(HttpClient httpClient, IOptions<ApiSettings> apiSettings)
        {
            //_httpClient = httpClient;
            var unsafeHttpClient = new UnsafeHttpClientHandler();
            _httpClient = new HttpClient(unsafeHttpClient);
            _apiBaseUrl = apiSettings.Value.DevBaseUrl;
        }

        public async Task<List<string>> GetSeriesListAsync(string sportName)
        {
            if(string.IsNullOrEmpty(sportName))
            {
                return new List<string>();
            }
            try
            {
                var response = await _httpClient.GetAsync($"{_apiBaseUrl}Serie/GetSeriesBySport?sportName={sportName}");
                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync();
                var series = JsonConvert.DeserializeObject<List<Series>>(json);
                var seriesList = new List<string>();
                foreach(var s in series)
                {
                    seriesList.Add(s.SerieName);
                }
                return seriesList;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error fetching series: {ex.Message}");
                return new List<string>();
            }
        }   
        
        public async Task<List<string>> GetSportListAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_apiBaseUrl}Serie/GetSports");
                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync();
                var sport = JsonConvert.DeserializeObject<List<Sport>>(json);
                var sportList = new List<string>();
                foreach(var s in sport)
                {
                    sportList.Add(s.SportName);
                }
                return sportList;
            }
            catch(Exception ex)
            {
                Debug.WriteLine($"Error fetching sports: {ex.Message}");
                return new List<string>();
            }
        }
    }
}
