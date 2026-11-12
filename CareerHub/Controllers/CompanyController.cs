using System.Security.Claims;
using CareerHub.Helpers;
using CareerHub.Models;
using CareerHub.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CareerHub.Controllers
{
    /// <summary>TV2 - Quản lý hồ sơ doanh nghiệp (bảng CONGTY).</summary>
    [Authorize(Roles = "Employer,Nhà tuyển dụng")]
    public class CompanyController : Controller
    {
        private readonly CareerHubContext _db;
        private readonly IWebHostEnvironment _env;

        public CompanyController(CareerHubContext db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }

        private int CurrentUserId =>
            int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : 0;

        // GET: /Company/Profile
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var company = await _db.Congties.AsNoTracking()
                .FirstOrDefaultAsync(c => c.MaNhaTuyenDung == CurrentUserId);

            // Chưa có công ty -> hiện form trống để tạo mới
            var vm = company == null
                ? new CompanyProfileVm()
                : new CompanyProfileVm
                {
                    MaCongTy = company.MaCongTy,
                    TenCongTy = company.TenCongTy,
                    Website = company.Website,
                    DiaChi = company.DiaChi,
                    QuyMo = company.QuyMo,
                    MoTa = company.MoTa,
                    Logo = company.Logo,
                    DaXacMinh = company.DaXacMinh
                };
            return View(vm);
        }

        // POST: /Company/Profile
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(CompanyProfileVm vm)
        {
            var company = await _db.Congties.FirstOrDefaultAsync(c => c.MaNhaTuyenDung == CurrentUserId);
            vm.Logo = company?.Logo; // giữ logo cũ để hiển thị lại khi form lỗi
            vm.DaXacMinh = company?.DaXacMinh;

            // Kiểm tra bảo mật file logo (đuôi file, magic bytes, <= 5MB)
            if (vm.LogoFile != null && vm.LogoFile.Length > 0)
            {
                var error = await UploadSecurity.ValidateImageAsync(vm.LogoFile);
                if (error != null) ModelState.AddModelError(nameof(vm.LogoFile), error);
            }

            if (!ModelState.IsValid) return View(vm);

            if (company == null)
            {
                company = new Congty
                {
                    MaNhaTuyenDung = CurrentUserId,
                    NgayTao = DateTime.Now,
                    DaXacMinh = false
                };
                _db.Congties.Add(company);
            }

            company.TenCongTy = vm.TenCongTy.Trim();
            company.Website = vm.Website?.Trim();
            company.DiaChi = vm.DiaChi?.Trim();
            company.QuyMo = vm.QuyMo?.Trim();
            company.MoTa = vm.MoTa?.Trim();

            if (vm.LogoFile != null && vm.LogoFile.Length > 0)
            {
                var oldLogo = company.Logo;
                company.Logo = await UploadSecurity.SaveImageAsync(vm.LogoFile, _env.WebRootPath, "uploads/logos");
                UploadSecurity.DeleteUploadedFile(oldLogo, _env.WebRootPath);
            }

            await _db.SaveChangesAsync();
            TempData["Success"] = "Đã lưu thông tin công ty.";
            return RedirectToAction(nameof(Profile));
        }
    }
}
