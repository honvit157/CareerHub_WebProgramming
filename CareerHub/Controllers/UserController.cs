using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CareerHub.Models;

namespace CareerHub.Controllers
{
    public class UserController : Controller
    {
        private readonly CareerHubContext db;

        public UserController(CareerHubContext context)
        {
            db = context;
        }

        // Hiển thị danh sách tài khoản
        // Hiển thị danh sách tài khoản, tìm kiếm và lọc
        public async Task<IActionResult> Index(string tuKhoa, string trangThai)
        {
            // Lưu lại giá trị tìm kiếm để hiển thị trên giao diện
            ViewBag.TuKhoa = tuKhoa;
            ViewBag.TrangThai = trangThai;

            // Lấy danh sách tài khoản
            var dsNguoiDung = db.Nguoidungs
                .OrderByDescending(n => n.NgayTao)
                .AsQueryable();

            // Tìm kiếm theo họ tên hoặc email
            if (!string.IsNullOrWhiteSpace(tuKhoa))
            {
                tuKhoa = tuKhoa.Trim();

                dsNguoiDung = dsNguoiDung.Where(n =>
                    (n.HoTen != null && n.HoTen.Contains(tuKhoa)) ||
                    (n.Email != null && n.Email.Contains(tuKhoa))
                );
            }

            // Lọc theo trạng thái tài khoản
            if (trangThai == "true")
            {
                dsNguoiDung = dsNguoiDung.Where(n => n.TrangThai == true);
            }
            else if (trangThai == "false")
            {
                dsNguoiDung = dsNguoiDung.Where(n => n.TrangThai == false);
            }

            // Lấy dữ liệu
            var ketQua = await dsNguoiDung.ToListAsync();

            return View(ketQua);
        }

        // Khóa hoặc mở khóa tài khoản
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CapNhatTrangThai(
            int maNguoiDung, bool trangThai)
        {
            var nguoiDung = await db.Nguoidungs
                .FindAsync(maNguoiDung);

            if (nguoiDung == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Không tìm thấy tài khoản."
                });
            }

            nguoiDung.TrangThai = trangThai;

            await db.SaveChangesAsync();

            return Json(new
            {
                success = true,
                message = trangThai
                    ? "Đã mở khóa tài khoản."
                    : "Đã khóa tài khoản.",
                trangThai = trangThai
            });
        }
    }
}