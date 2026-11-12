using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CareerHub.Models;

namespace CareerHub.Controllers
{
    [Authorize(Roles = "Quản trị viên,Admin")]
    public class AdminController : Controller
    {
        private readonly CareerHubContext db;
        public AdminController(CareerHubContext context)
        {
            db = context;
        }


        public async Task<IActionResult> Index(string tuKhoa)
        {
            // Thống kê tổng số tin tuyển dụng
            ViewBag.TongTinTuyenDung = await db.Tintuyendungs.CountAsync();

            // Thống kê số tin đang chờ duyệt
            ViewBag.TinChoDuyet = await db.Tintuyendungs
                .CountAsync(t => t.TrangThai == "Chờ duyệt");

            // Thống kê tổng số công ty
            ViewBag.TongCongTy = await db.Congties.CountAsync();

            // Thống kê tổng số lượt ứng tuyển
            ViewBag.TongLuotUngTuyen = await db.Ungtuyens.CountAsync();

            // Lưu từ khóa để hiển thị lại trên giao diện
            ViewBag.TuKhoa = tuKhoa;

            // Lấy danh sách tin chờ duyệt
            var dsTinChoDuyet = db.Tintuyendungs
                .Include(t => t.MaCongTyNavigation)
                .Where(t => t.TrangThai == "Chờ duyệt");

            // Tìm kiếm theo tiêu đề hoặc tên công ty
            if (!string.IsNullOrWhiteSpace(tuKhoa))
            {
                tuKhoa = tuKhoa.Trim();

                dsTinChoDuyet = dsTinChoDuyet.Where(t =>
                    (t.TieuDe != null && t.TieuDe.Contains(tuKhoa)) ||
                    (t.MaCongTyNavigation != null &&
                     t.MaCongTyNavigation.TenCongTy != null &&
                     t.MaCongTyNavigation.TenCongTy.Contains(tuKhoa))
                );
            }

            var ketQua = await dsTinChoDuyet.ToListAsync();

            return View(ketQua);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CapNhatTrangThai(int maTin, string trangThai)
        {
            if (trangThai != "Đã duyệt" && trangThai != "Từ chối")
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Trạng thái không hợp lệ."
                });
            }

            var tin = await db.Tintuyendungs.FindAsync(maTin);

            if (tin == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Không tìm thấy tin tuyển dụng."
                });
            }

            if (tin.TrangThai != "Chờ duyệt")
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Tin tuyển dụng đã được xử lý."
                });
            }

            tin.TrangThai = trangThai;

            await db.SaveChangesAsync();

            return Json(new
            {
                success = true,
                message = "Cập nhật trạng thái thành công.",
                trangThai = trangThai
            });
        }


    }
}
