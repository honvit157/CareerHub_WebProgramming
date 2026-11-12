using System.Security.Claims;
using CareerHub.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CareerHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobApiController : ControllerBase
    {
        private readonly CareerHubContext _context;

        public JobApiController(CareerHubContext context)
        {
            _context = context;
        }

        [HttpGet("{maDanhMuc}")]
        public IActionResult GetJobsByDanhMuc(string maDanhMuc)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int? currentUserId = string.IsNullOrEmpty(userIdString) ? null : int.Parse(userIdString);

            var savedJobIds = currentUserId.HasValue
                ? _context.Vieclamluus.Where(v => v.MaUngVien == currentUserId.Value).Select(v => v.MaTin).ToHashSet()
                : new HashSet<int>();

            var list = _context.Tintuyendungs
                .Include(t => t.MaCongTyNavigation)
                .Where(t => t.MaDanhMuc == maDanhMuc)
                .Select(t => new
                {
                    maTin = t.MaTin,
                    tieuDe = t.TieuDe,
                    tenCongTy = t.MaCongTyNavigation.TenCongTy,
                    diaDiem = t.DiaChiLamViec ?? "Toàn quốc",
                    mucLuong = t.LuongTu.HasValue
                        ? (t.LuongTu / 1000000) + " - " + (t.LuongDen / 1000000) + " triệu"
                        : "Thỏa thuận",
                    loaiHinh = t.LoaiHinh ?? "Full-time",
                    kinhNghiem = t.KinhNghiem ?? "Không yêu cầu",
                    hanNop = t.HanNop.HasValue ? t.HanNop.Value.ToString("dd/MM/yyyy") : "Không giới hạn",
                    isSaved = savedJobIds.Contains(t.MaTin)
                })
                .ToList();

            return Ok(list);
        }

        // API Thả tim / Bỏ tim (Lưu việc làm AJAX POST)
        [HttpPost("toggle-save/{maTin}")]
        public async Task<IActionResult> ToggleSaveJob(int maTin)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString))
            {
                return Unauthorized(new { success = false, message = "Bạn cần đăng nhập tài khoản Ứng viên để lưu việc làm!" });
            }
            int userId = int.Parse(userIdString);

            var daLuu = await _context.Vieclamluus
                .FirstOrDefaultAsync(v => v.MaUngVien == userId && v.MaTin == maTin);

            if (daLuu != null)
            {
                _context.Vieclamluus.Remove(daLuu);
                await _context.SaveChangesAsync();
                return Ok(new { success = true, isSaved = false, message = "Đã bỏ lưu việc làm!" });
            }
            else
            {
                var viecLuuMoi = new Vieclamluu
                {
                    MaUngVien = userId,
                    MaTin = maTin,
                    NgayLuu = DateTime.Now
                };
                _context.Vieclamluus.Add(viecLuuMoi);
                await _context.SaveChangesAsync();
                return Ok(new { success = true, isSaved = true, message = "Đã lưu việc làm vào danh sách yêu thích!" });
            }
        }
    }
}
