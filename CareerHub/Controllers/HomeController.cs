using CareerHub.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace CareerHub.Controllers
{
    public class HomeController : Controller
    {
        private readonly CareerHubContext _context;

        public HomeController(CareerHubContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Danh sách Danh mục cho khu vực AJAX lọc việc làm của M1
            var danhMucList = await _context.Danhmucs.ToListAsync();

            // Việc làm mới nhất cho ViewComponent hoặc hiển thị thêm nếu cần
            var latestJobs = await _context.Tintuyendungs
                .AsNoTracking()
                .Include(t => t.MaCongTyNavigation)
                .OrderByDescending(t => t.NgayDang)
                .ThenByDescending(t => t.MaTin)
                .Take(6)
                .ToListAsync();

            ViewData["LatestJobs"] = latestJobs;
            return View(danhMucList);
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
