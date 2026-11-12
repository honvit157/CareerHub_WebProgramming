using System.Security.Claims;
using CareerHub.Models;
using CareerHub.Models.ModelViews;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CareerHub.Models.ViewModels;

namespace CareerHub.Controllers
{
    public class AccountController : Controller
    {
        private readonly CareerHubContext _context;

        // Tiêm DbContext vào để lấy dữ liệu từ SQL
        public AccountController(CareerHubContext context)
        {
            _context = context;
        }

        // ==========================================
        // 1. TRANG HIỂN THỊ FORM ĐĂNG NHẬP (GET)
        // ==========================================
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }


        // ==========================================
        // 2. XỬ LÝ KHI NGƯỜI DÙNG BẤM NÚT ĐĂNG NHẬP (POST)
        // ==========================================
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            // Nếu người dùng để trống email hoặc mật khẩu -> Bắt nhập lại
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Tìm trong CSDL xem có ai trùng Email và Mật khẩu không
            // Dùng Include để kéo luôn bảng VAITRO đi kèm (để biết vai trò là gì)
            var user = await _context.Nguoidungs
                .Include(u => u.MaVaiTroNavigation)
                .FirstOrDefaultAsync(u => u.Email == model.Email && u.MatKhau == model.MatKhau);

            if (user == null)
            {
                ModelState.AddModelError("", "Email hoặc mật khẩu không chính xác.");
                return View(model);
            }

            // -------------------------------------------------------------
            // ĐOẠN NÀY LÀ CẤP "VÉ VÀO CỔNG" (CLAIMS) - BẠN ĐỌC KỸ CHỖ NÀY NHÉ:
            // -------------------------------------------------------------

            // 1. Ghi thông tin người dùng lên một tờ giấy (Claim)
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.MaNguoiDung.ToString()), // Lưu ID
                new Claim(ClaimTypes.Name, user.HoTen),                             // Lưu Họ tên để tí nữa chào: "Xin chào, Nam"
                new Claim(ClaimTypes.Email, user.Email),                            // Lưu Email
                new Claim(ClaimTypes.Role, user.MaVaiTroNavigation.TenVaiTro)       // LƯU VAI TRÒ: "Ứng viên" hoặc "Nhà tuyển dụng"
            };

            // 2. Gom các thông tin trên thành một tấm thẻ căn cước (Identity)
            var identity = new ClaimsIdentity(claims, "Cookies");

            // 3. Đưa thẻ cho trình duyệt giữ (Lưu vào Cookie)
            // Lần sau khi lướt web, trình duyệt tự chìa thẻ này ra
            await HttpContext.SignInAsync("Cookies", new ClaimsPrincipal(identity));

            // Đăng nhập thành công -> Về trang chủ
            return RedirectToAction("Index", "Home");

        }
        // ==========================================
        // 3. ĐĂNG XUẤT (LOGOUT)
        // ==========================================
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            // Xóa Cookie ở trình duyệt -> Trở về trạng thái chưa đăng nhập
            await HttpContext.SignOutAsync("Cookies");
            return RedirectToAction("Index", "Home");
        }

        // ==========================================
        // 4. TRANG BÁO LỖI KHÔNG ĐỦ QUYỀN TRUY CẬP
        // ==========================================
        [HttpGet]
        public IActionResult Denied()
        {
            return View();
        }

        // ==========================================
        // 1. HIỂN THỊ FORM ĐĂNG KÝ (GET)
        // ==========================================
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // ==========================================
        // 2. XỬ LÝ LƯU NGƯỜI DÙNG MỚI VÀO CSDL (POST)
        // ==========================================
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            // Kiểm tra các ràng buộc: để trống, mật khẩu không khớp...
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Kiểm tra xem Email này đã có ai đăng ký trước đó chưa
            bool emailDaTonTai = await _context.Nguoidungs.AnyAsync(u => u.Email == model.Email);
            if (emailDaTonTai)
            {
                ModelState.AddModelError("Email", "Email này đã được sử dụng. Vui lòng chọn email khác.");
                return View(model);
            }

            // Chuyển dữ liệu từ ViewModel sang Entity Model để lưu vào Database
            var nguoiDungMoi = new Nguoidung
            {
                HoTen = model.HoTen,
                Email = model.Email,
                MatKhau = model.MatKhau,
                DienThoai = model.SoDienThoai,
                MaVaiTro = model.MaVaiTro, // 3: Ứng viên, 2: Nhà tuyển dụng
                TrangThai = true,          // Tài khoản được kích hoạt luôn
                NgayTao = DateTime.Now
            };

            // Lưu xuống SQL Server
            _context.Nguoidungs.Add(nguoiDungMoi);
            await _context.SaveChangesAsync();

            // Gửi thông báo thành công sang trang Đăng nhập
            TempData["SuccessMessage"] = "Đăng ký tài khoản thành công! Vui lòng đăng nhập.";
            return RedirectToAction("Login");
        }
    }
}
    