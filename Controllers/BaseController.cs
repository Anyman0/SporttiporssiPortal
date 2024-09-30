using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SporttiporssiPortal.Interfaces;

namespace SporttiporssiPortal.Controllers
{
    public class BaseController : Controller
    {
        private readonly ISeriesService _seriesService;
        
        public BaseController(ISeriesService seriesService)
        {           
            _seriesService = seriesService;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var sportlistTask = _seriesService.GetSportListAsync();
            sportlistTask.Wait();
            ViewBag.SportList = sportlistTask.Result;
            var selectedSport = HttpContext.Session.GetString("SelectedSport");
            var seriesListTask = _seriesService.GetSeriesListAsync(selectedSport);
            seriesListTask.Wait();
            ViewBag.SeriesList = seriesListTask.Result;
            base.OnActionExecuting(context);
        }
    }
}
