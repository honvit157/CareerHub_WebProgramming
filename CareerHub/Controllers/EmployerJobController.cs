using System.Security.Claims;
using CareerHub.Models;
using CareerHub.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CareerHub.Controllers
{
    /// <summary>
    /// TV2 - Nhà tuyển dụng: CRUD tin tuyển dụng, xem danh sách ứng viên (ATS),
    /// đổi trạng thái hồ sơ bằng AJAX, xem / tải CV (PDF).
    /// </summary>
    [Authorize(Roles = "Employer")]
    public class EmployerJobController : Controller
    {
        private const string StatusPending = "Chờ duyệt";
        private const string StatusClosed = "Đã đóng";

        private readonly CareerHubContext _db;
        private readonly IWebHostEnvironment _env;

        public EmployerJobController(CareerHubContext db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }

        private int CurrentUserId =>
            int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : 0;

        private Task<Congty?> GetMyCompanyAsync() =>
            _db.Congties.FirstOrDefaultAsync(c => c.MaNhaTuyenDung == CurrentUserId);

        // Tin phải thuộc công ty của nhà tuyển dụng đang đăng nhập (chống sửa/xem tin của người khác)
        private IQueryable<Tintuyendung> MyJobs() =>
            _db.Tintuyendungs.Where(t => t.MaCongTyNavigation.MaNhaTuyenDung == CurrentUserId);

        // ===================== DANH SÁCH TIN CỦA CÔNG TY =====================
        public async Task<IActionResult> Index()
        {
            var company = await GetMyCompanyAsync();
            if (company == null)
            {
                TempData["Error"] = "Bạn cần cập nhật thông tin công ty trước khi đăng tin.";
                return RedirectToAction("Profile", "Company");
            }

            var jobs = await MyJobs()
                .Include(t => t.MaDanhMucNavigation)
                .Include(t => t.Ungtuyens)
                .AsNoTracking()
                .OrderByDescending(t => t.NgayDang)
                .ToListAsync();

            return View(jobs);
        }

        // ===================== THÊM TIN =====================
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            if (await GetMyCompanyAsync() == null)
            {
                TempData["Error"] = "Bạn cần cập nhật thông tin công ty trước khi đăng tin.";
                return RedirectToAction("Profile", "Company");
            }
            var vm = new JobFormVm();
            await FillListsAsync(vm);
            return View(vm);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(JobFormVm vm)
        {
            var company = await GetMyCompanyAsync();
            if (company == null) return RedirectToAction("Profile", "Company");

            if (!await CategoryExistsAsync(vm.MaDanhMuc))
                ModelState.AddModelError(nameof(vm.MaDanhMuc), "Ngành nghề không hợp lệ");

            if (!ModelState.IsValid)
            {
                await FillListsAsync(vm);
                return View(vm);
            }

            var job = new Tintuyendung
            {
                MaCongTy = company.MaCongTy,
                NgayDang = DateTime.Now,
                SoLuotXem = 0,
                TrangThai = StatusPending // chờ Admin (TV3) kiểm duyệt
            };
            await MapToEntityAsync(vm, job);

            _db.Tintuyendungs.Add(job);
            await _db.SaveChangesAsync();

            TempData["Success"] = "Đã đăng tin. Tin sẽ hiển thị sau khi Quản trị viên phê duyệt.";
            return RedirectToAction(nameof(Index));
        }

        // ===================== SỬA TIN =====================
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var job = await MyJobs().Include(t => t.MaKyNangs).AsNoTracking()
                .FirstOrDefaultAsync(t => t.MaTin == id);
            if (job == null) return NotFound();

            var vm = new JobFormVm
            {
                MaTin = job.MaTin,
                TieuDe = job.TieuDe,
                MaDanhMuc = job.MaDanhMuc?.Trim() ?? "",
                MoTaCv = job.MoTaCv,
                YeuCau = job.YeuCau,
                QuyenLoi = job.QuyenLoi,
                DiaChiLamViec = job.DiaChiLamViec,
                LuongTu = job.LuongTu,
                LuongDen = job.LuongDen,
                LoaiHinh = job.LoaiHinh,
                KinhNghiem = job.KinhNghiem,
                HanNop = job.HanNop,
                SelectedSkills = job.MaKyNangs.Select(k => k.MaKyNang.Trim()).ToList()
            };
            await FillListsAsync(vm);
            return View(vm);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, JobFormVm vm)
        {
            if (id != vm.MaTin) return BadRequest();

            var job = await MyJobs().Include(t => t.MaKyNangs).FirstOrDefaultAsync(t => t.MaTin == id);
            if (job == null) return NotFound();

            if (!await CategoryExistsAsync(vm.MaDanhMuc))
                ModelState.AddModelError(nameof(vm.MaDanhMuc), "Ngành nghề không hợp lệ");

            // Khi sửa tin cũ, hạn nộp đã nằm trong quá khứ thì không bắt buộc đổi
            if (vm.HanNop.HasValue && job.HanNop.HasValue && vm.HanNop.Value.Date == job.HanNop.Value.Date)
                ModelState.Remove(nameof(vm.HanNop));

            if (!ModelState.IsValid)
            {
                await FillListsAsync(vm);
                return View(vm);
            }

            await MapToEntityAsync(vm, job);

            // Tin đã duyệt / bị từ chối mà được chỉnh sửa -> gửi lại Admin duyệt. Tin đã đóng giữ nguyên.
            if (job.TrangThai != StatusClosed) job.TrangThai = StatusPending;

            await _db.SaveChangesAsync();
            TempData["Success"] = "Đã cập nhật tin tuyển dụng.";
            return RedirectToAction(nameof(Index));
        }

        // ===================== ĐÓNG TIN =====================
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Close(int id)
        {
            var job = await MyJobs().FirstOrDefaultAsync(t => t.MaTin == id);
            if (job == null) return NotFound();

            job.TrangThai = StatusClosed;
            await _db.SaveChangesAsync();
            TempData["Success"] = "Đã đóng tin tuyển dụng.";
            return RedirectToAction(nameof(Index));
        }

        // ===================== DANH SÁCH ỨNG VIÊN CỦA 1 TIN =====================
        public async Task<IActionResult> Applicants(int id)
        {
            var job = await MyJobs().AsNoTracking().FirstOrDefaultAsync(t => t.MaTin == id);
            if (job == null) return NotFound();

            var applications = await _db.Ungtuyens
                .Include(u => u.MaUngVienNavigation)
                .Include(u => u.MaCvNavigation)
                .Where(u => u.MaTin == id)
                .AsNoTracking()
                .OrderByDescending(u => u.NgayNop)
                .ToListAsync();

            ViewBag.Job = job;
            return View(applications);
        }

        // ===================== AJAX: ĐỔI TRẠNG THÁI HỒ SƠ ỨNG VIÊN =====================
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, string status)
        {
            if (!ApplicationStatus.All.Contains(status))
                return BadRequest(new { success = false, message = "Trạng thái không hợp lệ" });

            // Chỉ được đổi trạng thái hồ sơ nộp vào tin của chính công ty mình
            var application = await _db.Ungtuyens
                .Include(u => u.MaTinNavigation)
                .FirstOrDefaultAsync(u => u.MaUngTuyen == id &&
                                          u.MaTinNavigation.MaCongTyNavigation.MaNhaTuyenDung == CurrentUserId);
            if (application == null)
                return NotFound(new { success = false, message = "Không tìm thấy hồ sơ ứng tuyển" });

            var changed = application.TrangThai != status;
            application.TrangThai = status;
            application.NgayDuyet = status == ApplicationStatus.Submitted ? null : DateTime.Now;

            // Gửi thông báo cho ứng viên (bảng THONGBAO) khi trạng thái thay đổi
            if (changed && status != ApplicationStatus.Submitted)
            {
                _db.Thongbaos.Add(new Thongbao
                {
                    MaNguoiDung = application.MaUngVien,
                    TieuDe = "Cập nhật trạng thái hồ sơ ứng tuyển",
                    NoiDung = $"Hồ sơ của bạn cho vị trí \"{Truncate(application.MaTinNavigation.TieuDe, 150)}\" đã chuyển sang trạng thái: {status}.",
                    DaDoc = false,
                    NgayTao = DateTime.Now
                });
            }

            await _db.SaveChangesAsync();
            return Json(new
            {
                success = true,
                status = application.TrangThai,
                badgeClass = ApplicationStatus.BadgeClass(application.TrangThai)
            });
        }

        // ===================== XEM / TẢI CV (PDF) =====================
        [HttpGet]
        public Task<IActionResult> ViewCv(int id) => ServeCvAsync(id, download: false);

        [HttpGet]
        public Task<IActionResult> DownloadCv(int id) => ServeCvAsync(id, download: true);

        private async Task<IActionResult> ServeCvAsync(int applicationId, bool download)
        {
            // Nhà tuyển dụng chỉ xem được CV của ứng viên đã nộp vào tin của công ty mình
            var cv = await _db.Ungtuyens
                .Where(u => u.MaUngTuyen == applicationId &&
                            u.MaTinNavigation.MaCongTyNavigation.MaNhaTuyenDung == CurrentUserId)
                .Select(u => new { u.MaCvNavigation.DuongDanFile, u.MaCvNavigation.TenCv, HoTen = u.MaUngVienNavigation.HoTen })
                .FirstOrDefaultAsync();
            if (cv == null) return NotFound();

            // Chống path traversal: file bắt buộc nằm trong wwwroot/uploads và có đuôi .pdf
            var uploadsRoot = Path.GetFullPath(Path.Combine(_env.WebRootPath, "uploads")) + Path.DirectorySeparatorChar;
            var fullPath = Path.GetFullPath(Path.Combine(_env.WebRootPath, cv.DuongDanFile.TrimStart('/', '\\')));
            if (!fullPath.StartsWith(uploadsRoot, StringComparison.OrdinalIgnoreCase) ||
                !string.Equals(Path.GetExtension(fullPath), ".pdf", StringComparison.OrdinalIgnoreCase) ||
                !System.IO.File.Exists(fullPath))
            {
                TempData["Error"] = "Không tìm thấy file CV trên máy chủ.";
                return RedirectToAction(nameof(Index));
            }

            if (!download) return PhysicalFile(fullPath, "application/pdf"); // xem trực tiếp trên trình duyệt

            var downloadName = $"CV_{cv.HoTen}.pdf".Replace(' ', '_');
            return PhysicalFile(fullPath, "application/pdf", downloadName);
        }

        // ===================== HÀM HỖ TRỢ =====================
        private static string Truncate(string s, int max) => s.Length <= max ? s : s[..max];

        private Task<bool> CategoryExistsAsync(string? maDanhMuc) =>
            _db.Danhmucs.AnyAsync(d => d.MaDanhMuc == maDanhMuc);

        private async Task FillListsAsync(JobFormVm vm)
        {
            vm.Categories = await _db.Danhmucs.AsNoTracking().OrderBy(d => d.TenDanhMuc)
                .Select(d => new SelectListItem { Value = d.MaDanhMuc, Text = d.TenDanhMuc })
                .ToListAsync();
            vm.AllSkills = await _db.Kynangs.AsNoTracking().OrderBy(k => k.TenKyNang).ToListAsync();
        }

        /// <summary>Gán dữ liệu form vào entity và đồng bộ bảng trung gian KYNANGTIN.</summary>
        private async Task MapToEntityAsync(JobFormVm vm, Tintuyendung job)
        {
            job.TieuDe = vm.TieuDe.Trim();
            job.MaDanhMuc = vm.MaDanhMuc;
            job.MoTaCv = vm.MoTaCv;
            job.YeuCau = vm.YeuCau;
            job.QuyenLoi = vm.QuyenLoi;
            job.DiaChiLamViec = vm.DiaChiLamViec?.Trim();
            job.LuongTu = vm.LuongTu;
            job.LuongDen = vm.LuongDen;
            job.LoaiHinh = JobFormVm.JobTypes.Contains(vm.LoaiHinh) ? vm.LoaiHinh : null;
            job.KinhNghiem = vm.KinhNghiem?.Trim();
            job.HanNop = vm.HanNop;

            // Chỉ nhận các mã kỹ năng thật sự tồn tại trong DB
            var selected = vm.SelectedSkills ?? new List<string>();
            var skills = await _db.Kynangs.Where(k => selected.Contains(k.MaKyNang)).ToListAsync();

            job.MaKyNangs.Clear();
            foreach (var s in skills) job.MaKyNangs.Add(s);
        }
    }
}
