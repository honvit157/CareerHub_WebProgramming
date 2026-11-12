using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CareerHub.Models;

namespace CareerHub.Controllers
{
    public class CategoryController : Controller
    {
        private readonly CareerHubContext db;

        public CategoryController(CareerHubContext context)
        {
            db = context;
        }

        // Hiển thị danh sách danh mục
        // Hiển thị danh sách danh mục và tìm kiếm
        public async Task<IActionResult> Index(string tuKhoa)
        {
            ViewBag.TuKhoa = tuKhoa;

            var dsDanhMuc = db.Danhmucs.AsQueryable();

            // Tìm kiếm theo mã hoặc tên danh mục
            if (!string.IsNullOrWhiteSpace(tuKhoa))
            {
                tuKhoa = tuKhoa.Trim();

                dsDanhMuc = dsDanhMuc.Where(d =>
                    d.MaDanhMuc.Contains(tuKhoa) ||
                    d.TenDanhMuc.Contains(tuKhoa)
                );
            }

            var ketQua = await dsDanhMuc.ToListAsync();

            return View(ketQua);
        }

        // Hiển thị form thêm danh mục
        public IActionResult Create()
        {
            return View();
        }

        // Xử lý thêm danh mục
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Danhmuc danhMuc)
        {
            if (string.IsNullOrWhiteSpace(danhMuc.MaDanhMuc) ||
                string.IsNullOrWhiteSpace(danhMuc.TenDanhMuc))
            {
                ModelState.AddModelError("", "Mã và tên danh mục không được để trống.");
                return View(danhMuc);
            }

            var tonTai = await db.Danhmucs
                .AnyAsync(d => d.MaDanhMuc == danhMuc.MaDanhMuc);

            if (tonTai)
            {
                ModelState.AddModelError("MaDanhMuc", "Mã danh mục đã tồn tại.");
                return View(danhMuc);
            }

            db.Danhmucs.Add(danhMuc);
            await db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // Hiển thị form sửa danh mục
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var danhMuc = await db.Danhmucs.FindAsync(id);

            if (danhMuc == null)
            {
                return NotFound();
            }

            return View(danhMuc);
        }

        // Xử lý sửa danh mục
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, Danhmuc danhMuc)
        {
            if (id != danhMuc.MaDanhMuc)
            {
                return NotFound();
            }

            if (string.IsNullOrWhiteSpace(danhMuc.TenDanhMuc))
            {
                ModelState.AddModelError("TenDanhMuc", "Tên danh mục không được để trống.");
                return View(danhMuc);
            }

            var danhMucCu = await db.Danhmucs.FindAsync(id);

            if (danhMucCu == null)
            {
                return NotFound();
            }

            danhMucCu.TenDanhMuc = danhMuc.TenDanhMuc;
            danhMucCu.MoTa = danhMuc.MoTa;

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

            var danhMuc = await db.Danhmucs
                .FirstOrDefaultAsync(d => d.MaDanhMuc == id);

            if (danhMuc == null)
            {
                return NotFound();
            }

            return View(danhMuc);
        }

        // Xử lý xóa danh mục
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var danhMuc = await db.Danhmucs.FindAsync(id);

            if (danhMuc == null)
            {
                return NotFound();
            }

            db.Danhmucs.Remove(danhMuc);
            await db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}