
using CareerHub.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CareerHub.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Route("api/jobs")]
    public class JobsApiController : ControllerBase
    {
        private readonly CareerHubContext _context;

        public JobsApiController(CareerHubContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetJobs(
            [FromQuery] string? keyword,
            [FromQuery] string? categoryId,
            [FromQuery] string? location,
            [FromQuery] decimal? minSalary,
            [FromQuery] decimal? maxSalary,
            [FromQuery] string[]? workType,
            [FromQuery] int page = 1)
        {
            const int pageSize = 6;

            var query = _context.Tintuyendungs
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(t =>
                    t.TieuDe.Contains(keyword) ||
                    (t.MaCongTyNavigation != null &&
                     t.MaCongTyNavigation.TenCongTy.Contains(keyword)));
            }

            if (!string.IsNullOrWhiteSpace(categoryId))
                query = query.Where(t => t.MaDanhMuc == categoryId);

            if (!string.IsNullOrWhiteSpace(location))
                query = query.Where(t =>
                    t.DiaChiLamViec != null &&
                    t.DiaChiLamViec.Contains(location));

            if (minSalary.HasValue)
                query = query.Where(t =>
                    t.LuongDen >= minSalary.Value);

            if (maxSalary.HasValue)
                query = query.Where(t =>
                    t.LuongTu <= maxSalary.Value);

            if (workType != null && workType.Length > 0)
                query = query.Where(t =>
                    workType.Contains(t.LoaiHinh!));

            int totalJobs = await query.CountAsync();

            int totalPages = (int)Math.Ceiling(
                totalJobs / (double)pageSize);

            page = Math.Max(1, page);

            if (totalPages > 0)
                page = Math.Min(page, totalPages);

            var jobs = await query
                .OrderByDescending(t => t.NgayDang)
                .ThenByDescending(t => t.MaTin)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(t => new
                {
                    maTin = t.MaTin,
                    tieuDe = t.TieuDe,
                    tenCongTy = t.MaCongTyNavigation != null
                        ? t.MaCongTyNavigation.TenCongTy
                        : "",
                    logo = t.MaCongTyNavigation != null
                        ? t.MaCongTyNavigation.Logo
                        : null,
                    diaChiLamViec = t.DiaChiLamViec,
                    loaiHinh = t.LoaiHinh,
                    luongTu = t.LuongTu,
                    luongDen = t.LuongDen,
                    hanNop = t.HanNop
                })
                .ToListAsync();

            return Ok(new
            {
                page,
                pageSize,
                totalJobs,
                totalPages,
                jobs
            });
        }

        [HttpGet("categories")]
        [HttpGet("/api/categories")]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await _context.Danhmucs
                .AsNoTracking()
                .OrderBy(d => d.TenDanhMuc)
                .Select(d => new
                {
                    maDanhMuc = d.MaDanhMuc,
                    tenDanhMuc = d.TenDanhMuc
                })
                .ToListAsync();

            return Ok(categories);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetJob(int id)
        {
            var job = await _context.Tintuyendungs
                .AsNoTracking()
                .Where(t => t.MaTin == id)
                .Select(t => new
                {
                    maTin = t.MaTin,
                    tieuDe = t.TieuDe,
                    tenCongTy = t.MaCongTyNavigation != null
                        ? t.MaCongTyNavigation.TenCongTy
                        : "",
                    diaChiLamViec = t.DiaChiLamViec,
                    loaiHinh = t.LoaiHinh,
                    kinhNghiem = t.KinhNghiem,
                    luongTu = t.LuongTu,
                    luongDen = t.LuongDen,
                    moTaCv = t.MoTaCv,
                    yeuCau = t.YeuCau,
                    quyenLoi = t.QuyenLoi,
                    hanNop = t.HanNop
                })
                .FirstOrDefaultAsync();

            if (job == null)
                return NotFound(new
                {
                    message = "Job not found"
                });

            return Ok(job);
        }
    }
}
