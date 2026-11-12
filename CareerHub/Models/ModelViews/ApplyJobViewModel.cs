using System.ComponentModel.DataAnnotations;
using CareerHub.Models;
using Microsoft.AspNetCore.Http;

namespace CareerHub.Models.ViewModels
{
    public class ApplyJobViewModel
    {
        // 1. Thông tin công việc ứng tuyển
        public int MaTin { get; set; }
        public string? TieuDeTin { get; set; }
        public string? TenCongTy { get; set; }

        // 2. Kiểu nộp: "CoSan" (Dùng hồ sơ có sẵn), "TaiMoi" (Tải file mới), hoặc "NhapTay" (Nhập tay thông tin)
        public string KieuNop { get; set; } = "CoSan";

        // 3. Nếu chọn dùng CV có sẵn:
        [Display(Name = "Chọn hồ sơ CV")]
        public int? MaCvCoSan { get; set; }

        // 4. Nếu chọn tải lên CV mới:
        [Display(Name = "Tên hồ sơ mới")]
        public string? TenCvMoi { get; set; }

        [Display(Name = "Tải file CV (PDF / Word)")]
        public IFormFile? FileCvMoi { get; set; }

        // 5. Nếu chọn Nhập tay trực tiếp (Giải pháp 2):
        [Display(Name = "Trình độ học vấn")]
        public string? HocVanNhapTay { get; set; }

        [Display(Name = "Kinh nghiệm làm việc")]
        public string? KinhNghiemNhapTay { get; set; }

        [Display(Name = "Kỹ năng nổi bật")]
        public string? KyNangNhapTay { get; set; }

        // 6. Thư giới thiệu gửi đến nhà tuyển dụng
        [Display(Name = "Thư giới thiệu / Lời nhắn")]
        public string? ThuGioiThieu { get; set; }

        // 7. Danh sách các CV hiện có của ứng viên để đổ vào Dropdown
        public List<Hosocv>? DanhSachCvCoSan { get; set; }
    }
}
