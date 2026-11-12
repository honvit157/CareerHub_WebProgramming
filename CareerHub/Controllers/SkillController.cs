using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CareerHub.Models;

namespace CareerHub.Controllers
{
    public class SkillController : Controller
    {
        private readonly CareerHubContext db;

        public SkillController(CareerHubContext context)
        {
            db = context;
        }

        // Hiển thị danh sách kỹ năng
        // Hiển thị danh sách kỹ năng và tìm kiếm
        public async Task<IActionResult> Index(string tuKhoa)
        {
            ViewBag.TuKhoa = tuKhoa;

            var dsKyNang = db.Kynangs.AsQueryable();

            // Tìm kiếm theo mã hoặc tên kỹ năng
            if (!string.IsNullOrWhiteSpace(tuKhoa))
            {
                tuKhoa = tuKhoa.Trim();

                dsKyNang = dsKyNang.Where(k =>
                    k.MaKyNang.Contains(tuKhoa) ||
                    k.TenKyNang.Contains(tuKhoa)
                );
            }

            var ketQua = await dsKyNang.ToListAsync();

            return View(ketQua);
        }
        // Hiển thị form thêm kỹ năng
        public IActionResult Create()
        {
            return View();
        }

        // Xử lý thêm kỹ năng
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Kynang kyNang)
        {
            if (string.IsNullOrWhiteSpace(kyNang.MaKyNang) ||
                string.IsNullOrWhiteSpace(kyNang.TenKyNang))
            {
                ModelState.AddModelError("", "Mã và tên kỹ năng không được để trống.");
                return View(kyNang);
            }

            var tonTai = await db.Kynangs
                .AnyAsync(k => k.MaKyNang == kyNang.MaKyNang);

            if (tonTai)
            {
                ModelState.AddModelError("MaKyNang", "Mã kỹ năng đã tồn tại.");
                return View(kyNang);
            }

            db.Kynangs.Add(kyNang);
            await db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // Hiển thị form sửa
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var kyNang = await db.Kynangs.FindAsync(id);

            if (kyNang == null)
            {
                return NotFound();
            }

            return View(kyNang);
        }

        // Xử lý sửa kỹ năng
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, Kynang kyNang)
        {
            if (id != kyNang.MaKyNang)
            {
                return NotFound();
            }

            if (string.IsNullOrWhiteSpace(kyNang.TenKyNang))
            {
                ModelState.AddModelError("TenKyNang", "Tên kỹ năng không được để trống.");
                return View(kyNang);
            }

            var kyNangCu = await db.Kynangs.FindAsync(id);

            if (kyNangCu == null)
            {
                return NotFound();
            }

            kyNangCu.TenKyNang = kyNang.TenKyNang;

            await db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // Hiển thị xác nhận xóa
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var kyNang = await db.Kynangs
                .FirstOrDefaultAsync(k => k.MaKyNang == id);

            if (kyNang == null)
            {
                return NotFound();
            }

            return View(kyNang);
        }

        // Xử lý xóa
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var kyNang = await db.Kynangs.FindAsync(id);

            if (kyNang == null)
            {
                return NotFound();
            }

            db.Kynangs.Remove(kyNang);
            await db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}