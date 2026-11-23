using Microsoft.AspNetCore.Mvc;

using SigortaDernegi.ViewModels;

namespace SigortaDernegi.Controllers
{
    public class ReportController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View(new ConfidentialReportPageViewModel());
        }
    }
}
