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

        // 1. TRANG CHỦ: Hiển thị danh sách việc làm nổi bật/mới nhất
        public async Task<IActionResult> Index()
        {
            var latestJobs = await _context.Tintuyendungs
                .AsNoTracking()
                .Include(t => t.MaCongTyNavigation)
                .OrderByDescending(t => t.NgayDang)
                .ThenByDescending(t => t.MaTin)
                .Take(8)
                .ToListAsync();

            return View(latestJobs);
        }

        // 2. TRANG LỌC THEO NGÀNH NGHỀ BẰNG AJAX (Chức năng thi cử của M1)
        public async Task<IActionResult> FilterByCategory()
        {
            var danhMucList = await _context.Danhmucs.ToListAsync();
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
