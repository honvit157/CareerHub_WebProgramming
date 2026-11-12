using CareerHub.Models;
using CareerHub.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CareerHub.Controllers
{
    // KHÓA CỬA BẢO VỆ: Chỉ có tài khoản vai trò "Ứng viên" mới được phép vào Controller này!
    [Authorize(Roles = "Ứng viên")]
    public class CandidateController : Controller
    {
        private readonly CareerHubContext _context;

        public CandidateController(CareerHubContext context)
        {
            _context = context;
        }

        // ==========================================
        // 1. HỒ SƠ TRỰC TUYẾN CÁ NHÂN (GET & POST) - GIẢI PHÁP 1
        // ==========================================
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdString == null) return RedirectToAction("Login", "Account");
            int userId = int.Parse(userIdString);

            var user = await _context.Nguoidungs.FirstOrDefaultAsync(u => u.MaNguoiDung == userId);
            if (user == null) return NotFound();

            // Lấy hồ sơ online (nếu đã có) từ bảng HOSOCV
            var onlineCv = await _context.Hosocvs
                .FirstOrDefaultAsync(c => c.MaUngVien == userId && c.MacDinh == true);

            var model = new ProfileViewModel
            {
                MaNguoiDung = user.MaNguoiDung,
                HoTen = user.HoTen,
                Email = user.Email,
                DienThoai = user.DienThoai,
                DiaChi = user.DiaChi,
                ViTriMongMuon = onlineCv?.TenCv,
                GioiThieuBanThan = onlineCv?.TieuSu
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(ProfileViewModel model)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdString == null) return RedirectToAction("Login", "Account");
            int userId = int.Parse(userIdString);

            var user = await _context.Nguoidungs.FirstOrDefaultAsync(u => u.MaNguoiDung == userId);
            if (user == null) return NotFound();

            // Cập nhật thông tin tài khoản
            user.HoTen = model.HoTen.Trim();
            user.DienThoai = model.DienThoai?.Trim();
            user.DiaChi = model.DiaChi?.Trim();

            // Tìm hồ sơ online trong HOSOCV
            var onlineCv = await _context.Hosocvs
                .FirstOrDefaultAsync(c => c.MaUngVien == userId && c.MacDinh == true);

            string fullBio = "";
            if (!string.IsNullOrWhiteSpace(model.GioiThieuBanThan)) fullBio += $"[Giới thiệu]: {model.GioiThieuBanThan.Trim()}\n";
            if (!string.IsNullOrWhiteSpace(model.HocVan)) fullBio += $"[Học vấn]: {model.HocVan.Trim()}\n";
            if (!string.IsNullOrWhiteSpace(model.KinhNghiemLamViec)) fullBio += $"[Kinh nghiệm]: {model.KinhNghiemLamViec.Trim()}\n";
            if (!string.IsNullOrWhiteSpace(model.KyNang)) fullBio += $"[Kỹ năng]: {model.KyNang.Trim()}";

            if (onlineCv != null)
            {
                onlineCv.TenCv = string.IsNullOrWhiteSpace(model.ViTriMongMuon) ? "Hồ sơ trực tuyến của tôi" : model.ViTriMongMuon.Trim();
                onlineCv.TieuSu = fullBio;
                onlineCv.NgayCapNhat = DateTime.Now;
            }
            else
            {
                onlineCv = new Hosocv
                {
                    MaUngVien = userId,
                    TenCv = string.IsNullOrWhiteSpace(model.ViTriMongMuon) ? "Hồ sơ trực tuyến của tôi" : model.ViTriMongMuon.Trim(),
                    TieuSu = fullBio,
                    DuongDanFile = "#online_profile",
                    MacDinh = true,
                    NgayCapNhat = DateTime.Now
                };
                _context.Hosocvs.Add(onlineCv);
            }

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Cập nhật hồ sơ trực tuyến thành công!";
            return RedirectToAction("Profile");
        }

        // ==========================================
        // 2. TRANG XEM DANH SÁCH FILE CV (GET, UPLOAD, DELETE)
        // ==========================================
        [HttpGet]
        public async Task<IActionResult> MyCV()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdString == null) return RedirectToAction("Login", "Account");
            int userId = int.Parse(userIdString);

            var danhSachCV = await _context.Hosocvs
                .Where(c => c.MaUngVien == userId)
                .OrderByDescending(c => c.NgayCapNhat)
                .ToListAsync();

            return View(danhSachCV);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadCV(string tenCv, IFormFile fileCv)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdString == null) return RedirectToAction("Login", "Account");
            int userId = int.Parse(userIdString);

            if (fileCv == null || fileCv.Length == 0)
            {
                TempData["ErrorMessage"] = "Vui lòng chọn file CV từ máy tính!";
                return RedirectToAction("MyCV");
            }

            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "cvs");
            if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(fileCv.FileName);
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await fileCv.CopyToAsync(stream);
            }

            var newCv = new Hosocv
            {
                MaUngVien = userId,
                TenCv = string.IsNullOrWhiteSpace(tenCv) ? fileCv.FileName : tenCv.Trim(),
                DuongDanFile = "/uploads/cvs/" + fileName,
                MacDinh = false,
                NgayCapNhat = DateTime.Now
            };

            _context.Hosocvs.Add(newCv);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Tải lên hồ sơ CV mới thành công!";
            return RedirectToAction("MyCV");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCV(int id)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdString == null) return RedirectToAction("Login", "Account");
            int userId = int.Parse(userIdString);

            var cv = await _context.Hosocvs.FirstOrDefaultAsync(c => c.MaCv == id && c.MaUngVien == userId);
            if (cv == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy hồ sơ CV!";
                return RedirectToAction("MyCV");
            }

            bool daDungNopDon = await _context.Ungtuyens.AnyAsync(u => u.MaCv == id);
            if (daDungNopDon)
            {
                TempData["ErrorMessage"] = "Không thể xóa CV này vì bạn đã dùng nó để ứng tuyển việc làm!";
                return RedirectToAction("MyCV");
            }

            _context.Hosocvs.Remove(cv);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Đã xóa hồ sơ CV thành công!";
            return RedirectToAction("MyCV");
        }

        // ==========================================
        // 3. XỬ LÝ NỘP HỒ SƠ ỨNG TUYỂN (POST) - HỖ TRỢ CẢ 3 KIỂU
        // ==========================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Apply(ApplyJobViewModel model)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdString == null) return RedirectToAction("Login", "Account");
            int userId = int.Parse(userIdString);

            bool daNopDon = await _context.Ungtuyens.AnyAsync(u => u.MaTin == model.MaTin && u.MaUngVien == userId);
            if (daNopDon)
            {
                TempData["ErrorMessage"] = "Bạn đã nộp hồ sơ vào vị trí công việc này rồi!";
                return RedirectToAction("MyApplications");
            }

            int maCvDuocChon = 0;
            string thuGioiThieuFinal = model.ThuGioiThieu?.Trim() ?? "";

            // KIỂU 1: Dùng CV có sẵn / Hồ sơ trực tuyến
            if (model.KieuNop == "CoSan")
            {
                if (!model.MaCvCoSan.HasValue)
                {
                    TempData["ErrorMessage"] = "Vui lòng chọn một hồ sơ CV để nộp!";
                    return RedirectToAction("MyApplications");
                }
                maCvDuocChon = model.MaCvCoSan.Value;
            }
            // KIỂU 2: Tải lên file CV mới
            else if (model.KieuNop == "TaiMoi")
            {
                if (model.FileCvMoi == null || model.FileCvMoi.Length == 0)
                {
                    TempData["ErrorMessage"] = "Vui lòng chọn file CV từ máy tính!";
                    return RedirectToAction("MyApplications");
                }

                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "cvs");
                if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(model.FileCvMoi.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.FileCvMoi.CopyToAsync(stream);
                }

                var newCv = new Hosocv
                {
                    MaUngVien = userId,
                    TenCv = string.IsNullOrWhiteSpace(model.TenCvMoi) ? model.FileCvMoi.FileName : model.TenCvMoi.Trim(),
                    DuongDanFile = "/uploads/cvs/" + fileName,
                    MacDinh = false,
                    NgayCapNhat = DateTime.Now
                };
                _context.Hosocvs.Add(newCv);
                await _context.SaveChangesAsync();

                maCvDuocChon = newCv.MaCv;
            }
            // KIỂU 3: Nhập tay trực tiếp (Giải pháp 2)
            else if (model.KieuNop == "NhapTay")
            {
                var newOnlineCv = new Hosocv
                {
                    MaUngVien = userId,
                    TenCv = "Hồ sơ ứng tuyển nhanh (" + DateTime.Now.ToString("dd/MM/yyyy HH:mm") + ")",
                    TieuSu = $"[Học vấn]: {model.HocVanNhapTay}\n[Kinh nghiệm]: {model.KinhNghiemNhapTay}\n[Kỹ năng]: {model.KyNangNhapTay}",
                    DuongDanFile = "#quick_apply",
                    MacDinh = false,
                    NgayCapNhat = DateTime.Now
                };
                _context.Hosocvs.Add(newOnlineCv);
                await _context.SaveChangesAsync();

                maCvDuocChon = newOnlineCv.MaCv;
                thuGioiThieuFinal = $"[Ứng tuyển nhanh]\n- Học vấn: {model.HocVanNhapTay}\n- Kinh nghiệm: {model.KinhNghiemNhapTay}\n- Kỹ năng: {model.KyNangNhapTay}\n- Lời nhắn: {thuGioiThieuFinal}";
            }

            var ungTuyen = new Ungtuyen
            {
                MaTin = model.MaTin,
                MaUngVien = userId,
                MaCv = maCvDuocChon,
                ThuGioiThieu = thuGioiThieuFinal,
                TrangThai = "Đang chờ duyệt",
                NgayNop = DateTime.Now
            };

            _context.Ungtuyens.Add(ungTuyen);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Nộp hồ sơ ứng tuyển thành công! Chúc bạn may mắn.";
            return RedirectToAction("MyApplications");
        }

        // ==========================================
        // 4. XEM LỊCH SỬ CÁC VIỆC LÀM ĐÃ NỘP (GET)
        // ==========================================
        [HttpGet]
        public async Task<IActionResult> MyApplications()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdString == null) return RedirectToAction("Login", "Account");
            int userId = int.Parse(userIdString);

            var danhSach = await _context.Ungtuyens
                .Include(u => u.MaTinNavigation)
                    .ThenInclude(t => t.MaCongTyNavigation)
                .Include(u => u.MaCvNavigation)
                .Where(u => u.MaUngVien == userId)
                .OrderByDescending(u => u.NgayNop)
                .ToListAsync();

            return View(danhSach);
        }

        // ==========================================
        // 5. DANH SÁCH VIỆC LÀM ĐÃ LƯU (GET)
        // ==========================================
        [HttpGet]
        public async Task<IActionResult> SavedJobs()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdString == null) return RedirectToAction("Login", "Account");
            int userId = int.Parse(userIdString);

            var danhSachLuu = await _context.Vieclamluus
                .Include(v => v.MaTinNavigation)
                    .ThenInclude(t => t.MaCongTyNavigation)
                .Where(v => v.MaUngVien == userId)
                .OrderByDescending(v => v.NgayLuu)
                .ToListAsync();

            return View(danhSachLuu);
        }

        // ==========================================
        // 6. LẤY DỮ LIỆU ĐỔ VÀO POPUP ỨNG TUYỂN (GET)
        // ==========================================
        [HttpGet]
        public async Task<IActionResult> GetApplyModal(int maTin)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdString == null) return Unauthorized();
            int userId = int.Parse(userIdString);

            var tin = await _context.Tintuyendungs
                .Include(t => t.MaCongTyNavigation)
                .FirstOrDefaultAsync(t => t.MaTin == maTin);

            if (tin == null) return NotFound();

            var danhSachCv = await _context.Hosocvs
                .Where(c => c.MaUngVien == userId)
                .ToListAsync();

            var viewModel = new ApplyJobViewModel
            {
                MaTin = tin.MaTin,
                TieuDeTin = tin.TieuDe,
                TenCongTy = tin.MaCongTyNavigation?.TenCongTy ?? "Công ty tuyển dụng",
                DanhSachCvCoSan = danhSachCv
            };

            return PartialView("_ApplyModal", viewModel);
        }
    }
}
