using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SporttiporssiPortal.Configurations;
using SporttiporssiPortal.Interfaces;
using SporttiporssiPortal.Models;
using System.Diagnostics;

namespace SporttiporssiPortal.Controllers
{
    public class SeriesController : BaseController
    {
        private readonly ISeriesService _seriesService;
        
        public SeriesController(ISeriesService seriesService) : base(seriesService)
        {
            _seriesService = seriesService;
        }

        [HttpPost]
        public IActionResult SetSeries(string selectedSeries)
        {
            if (!string.IsNullOrEmpty(selectedSeries))
            {
                // Save the selected series in the session
                HttpContext.Session.SetString("SelectedSeries", selectedSeries);
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public IActionResult SetSport(string selectedSport)
        {
            if(!string.IsNullOrEmpty(selectedSport))
            {
                // Save the selected sport in the session
                HttpContext.Session.SetString("SelectedSport", selectedSport);
                HttpContext.Session.Remove("SelectedSeries");
            }
            return RedirectToAction("Index", "Home");
        }
    }
}
