using System.Security.Claims;
using CareerHub.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CareerHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CandidateApiController : ControllerBase
    {
        private readonly CareerHubContext _context;

        public CandidateApiController(CareerHubContext context)
        {
            _context = context;
        }

        // 1. Lấy toàn bộ việc đã nộp của Ứng viên (Test: /api/candidateapi)
        [HttpGet]
        public async Task<IActionResult> GetAllApplications()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdString == null) return Unauthorized();
            int userId = int.Parse(userIdString);

            var list = await _context.Ungtuyens
                .Include(u => u.MaTinNavigation)
                    .ThenInclude(t => t.MaCongTyNavigation)
                .Include(u => u.MaCvNavigation)
                .Where(u => u.MaUngVien == userId)
                .OrderByDescending(u => u.NgayNop)
                .Select(u => new
                {
                    maUngTuyen = u.MaUngTuyen,
                    tieuDe = u.MaTinNavigation.TieuDe,
                    diaDiem = u.MaTinNavigation.DiaChiLamViec ?? "Toàn quốc",
                    tenCongTy = u.MaTinNavigation.MaCongTyNavigation.TenCongTy,
                    tenCv = u.MaCvNavigation.TenCv,
                    duongDanCv = u.MaCvNavigation.DuongDanFile,
                    ngayNop = u.NgayNop.HasValue ? u.NgayNop.Value.ToString("dd/MM/yyyy HH:mm") : "",
                    trangThai = u.TrangThai ?? "Đang chờ duyệt"
                })
                .ToListAsync();

            return Ok(list); // Trả về danh sách dạng JSON
        }

        // 2. Lọc việc đã nộp theo trạng thái (Test: /api/candidateapi/filter/Đang chờ duyệt)
        [HttpGet("filter/{trangThai}")]
        public async Task<IActionResult> GetByTrangThai(string trangThai)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdString == null) return Unauthorized();
            int userId = int.Parse(userIdString);

            var query = _context.Ungtuyens
                .Include(u => u.MaTinNavigation)
                    .ThenInclude(t => t.MaCongTyNavigation)
                .Include(u => u.MaCvNavigation)
                .Where(u => u.MaUngVien == userId);

            // Nếu không phải "all" thì lọc theo trạng thái
            if (trangThai != "all")
            {
                query = query.Where(u => u.TrangThai == trangThai);
            }

            var list = await query
                .OrderByDescending(u => u.NgayNop)
                .Select(u => new
                {
                    maUngTuyen = u.MaUngTuyen,
                    tieuDe = u.MaTinNavigation.TieuDe,
                    diaDiem = u.MaTinNavigation.DiaChiLamViec ?? "Toàn quốc",
                    tenCongTy = u.MaTinNavigation.MaCongTyNavigation.TenCongTy,
                    tenCv = u.MaCvNavigation.TenCv,
                    duongDanCv = u.MaCvNavigation.DuongDanFile,
                    ngayNop = u.NgayNop.HasValue ? u.NgayNop.Value.ToString("dd/MM/yyyy HH:mm") : "",
                    trangThai = u.TrangThai ?? "Đang chờ duyệt"
                })
                .ToListAsync();

            return Ok(list);
        }

    }
}