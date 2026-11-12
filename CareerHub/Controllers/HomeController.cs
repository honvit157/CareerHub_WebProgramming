using CareerHub.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace CareerHub.Controllers
{
    public class HomeController : Controller
    {
        private readonly CareerHubContext _context;

        // Tiêm DbContext vào để lấy dữ liệu từ CSDL
        public HomeController(CareerHubContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Lấy danh sách Danh mục ngành nghề từ CSDL truyền xuống View
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