using CareerHub.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
namespace CareerHub.Controllers
{
    public class HomeController : Controller
    {
        private readonly CareerHubContext _context;

        public HomeController(CareerHubContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var context = HttpContext.RequestServices
    .GetRequiredService<CareerHubContext>();

            var latestJobs = context.Tintuyendungs
                .AsNoTracking()
                .Include(t => t.MaCongTyNavigation)
                .OrderByDescending(t => t.NgayDang)
                .ThenByDescending(t => t.MaTin)
                .Take(6)
                .ToList();

            ViewData["LatestJobs"] = latestJobs;
            return View(latestJobs);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
