using System.ComponentModel.DataAnnotations;

namespace CareerHub.Models.ViewModels
{
    public class ProfileViewModel
    {
        public int MaNguoiDung { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập họ và tên")]
        [Display(Name = "Họ và tên")]
        public string HoTen { get; set; } = null!;

        [Required(ErrorMessage = "Vui lòng nhập email")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; } = null!;

        [Display(Name = "Số điện thoại")]
        public string? DienThoai { get; set; }

        [Display(Name = "Địa chỉ hiện tại")]
        public string? DiaChi { get; set; }

        // Phần hồ sơ online (lưu vào HOSOCV)
        [Display(Name = "Tiêu đề hồ sơ / Vị trí mong muốn")]
        public string? ViTriMongMuon { get; set; }

        [Display(Name = "Giới thiệu bản thân")]
        public string? GioiThieuBanThan { get; set; }

        [Display(Name = "Kinh nghiệm làm việc")]
        public string? KinhNghiemLamViec { get; set; }

        [Display(Name = "Học vấn / Trình độ chuyên môn")]
        public string? HocVan { get; set; }

        [Display(Name = "Kỹ năng nổi bật")]
        public string? KyNang { get; set; }
    }
}
