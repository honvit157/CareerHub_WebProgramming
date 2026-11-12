using System.Security.Claims;
using CareerHub.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CareerHub.Controllers
{
    /// <summary>TV5 - Quản lý thông báo của người dùng đang đăng nhập (bảng THONGBAO).</summary>
    [Authorize]
    public class NotificationController : Controller
    {
        private const int PageSize = 10;
        private readonly CareerHubContext _db;

        public NotificationController(CareerHubContext db) => _db = db;

        private int CurrentUserId =>
            int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : 0;

        // GET: /Notification  -> trang xem toàn bộ thông báo (có phân trang)
        public async Task<IActionResult> Index(int page = 1)
        {
            if (page < 1) page = 1;

            var query = _db.Thongbaos.AsNoTracking().Where(t => t.MaNguoiDung == CurrentUserId);
            var total = await query.CountAsync();
            var items = await query
                .OrderByDescending(t => t.NgayTao)
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

            ViewBag.Page = page;
            ViewBag.TotalPages = Math.Max(1, (int)Math.Ceiling(total / (double)PageSize));
            return View(items);
        }

        // ===================== AJAX: CHUÔNG THÔNG BÁO =====================

        // GET: /Notification/Latest -> 5 thông báo mới nhất + số thông báo chưa đọc
        [HttpGet]
        public async Task<IActionResult> Latest()
        {
            var mine = _db.Thongbaos.AsNoTracking().Where(t => t.MaNguoiDung == CurrentUserId);

            var unread = await mine.CountAsync(t => t.DaDoc != true);
            var items = await mine
                .OrderByDescending(t => t.NgayTao)
                .Take(5)
                .Select(t => new
                {
                    id = t.MaThongBao,
                    title = t.TieuDe,
                    content = t.NoiDung,
                    isRead = t.DaDoc == true,
                    time = t.NgayTao
                })
                .ToListAsync();

            return Json(new
            {
                unreadCount = unread,
                items = items.Select(i => new
                {
                    i.id,
                    i.title,
                    i.content,
                    i.isRead,
                    time = i.time?.ToString("dd/MM/yyyy HH:mm")
                })
            });
        }

        
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkAllRead()
        {
            await _db.Thongbaos
                .Where(t => t.MaNguoiDung == CurrentUserId && t.DaDoc != true)
                .ExecuteUpdateAsync(s => s.SetProperty(t => t.DaDoc, (bool?)true));

            return Json(new { success = true, unreadCount = 0 });
        }

        // POST: /Notification/MarkRead/5 -> đánh dấu 1 thông báo đã đọc
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkRead(int id)
        {
            // Chỉ được đổi thông báo của chính mình
            var updated = await _db.Thongbaos
                .Where(t => t.MaThongBao == id && t.MaNguoiDung == CurrentUserId)
                .ExecuteUpdateAsync(s => s.SetProperty(t => t.DaDoc, (bool?)true));
            if (updated == 0) return NotFound(new { success = false });

            var unread = await _db.Thongbaos.CountAsync(t => t.MaNguoiDung == CurrentUserId && t.DaDoc != true);
            return Json(new { success = true, unreadCount = unread });
        }
    }
}