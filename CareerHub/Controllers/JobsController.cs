
using CareerHub.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CareerHub.Controllers
{
    public class JobsController : Controller
    {
        private readonly CareerHubContext _context;

        public JobsController(CareerHubContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(
            string? keyword,
            string? categoryId,
            string? location,
            decimal? minSalary,
            decimal? maxSalary,
            string[]? workType,
            int page = 1)
        {
            const int pageSize = 6;

            var query = _context.Tintuyendungs
                .AsNoTracking()
                .Include(t => t.MaCongTyNavigation)
                .Include(t => t.MaDanhMucNavigation)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(t =>
                    t.TieuDe.Contains(keyword) ||
                    (t.MaCongTyNavigation != null &&
                     t.MaCongTyNavigation.TenCongTy.Contains(keyword)));
            }

            if (!string.IsNullOrWhiteSpace(categoryId))
            {
                query = query.Where(t =>
                    t.MaDanhMuc == categoryId);
            }

            if (!string.IsNullOrWhiteSpace(location))
            {
                query = query.Where(t =>
                    t.DiaChiLamViec != null &&
                    t.DiaChiLamViec.Contains(location));
            }

            if (minSalary.HasValue)
            {
                query = query.Where(t =>
                    t.LuongDen >= minSalary.Value);
            }

            if (maxSalary.HasValue)
            {
                query = query.Where(t =>
                    t.LuongTu <= maxSalary.Value);
            }

            if (workType != null && workType.Length > 0)
            {
                query = query.Where(t =>
                    workType.Contains(t.LoaiHinh!));
            }

            int totalJobs = await query.CountAsync();

            int totalPages = (int)Math.Ceiling(
                totalJobs / (double)pageSize);

            page = Math.Max(1, page);

            if (totalPages > 0)
            {
                page = Math.Min(page, totalPages);
            }

            var jobs = await query
                .OrderByDescending(t => t.NgayDang)
                .ThenByDescending(t => t.MaTin)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.Page = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalJobs = totalJobs;

            ViewBag.Keyword = keyword;
            ViewBag.CategoryId = categoryId;
            ViewBag.Location = location;
            ViewBag.MinSalary = minSalary;
            ViewBag.MaxSalary = maxSalary;
            ViewBag.WorkType = workType;

            return View(jobs);
        }

        public async Task<IActionResult> Details(int id)
        {
            var job = await _context.Tintuyendungs
                .Include(t => t.MaCongTyNavigation)
                .Include(t => t.MaDanhMucNavigation)
                .Include(t => t.MaKyNangs)
                .FirstOrDefaultAsync(t => t.MaTin == id);

            if (job == null)
            {
                return NotFound();
            }

            job.SoLuotXem = (job.SoLuotXem ?? 0) + 1;

            await _context.SaveChangesAsync();

            return View(job);
        }
    }
}
