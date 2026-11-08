using System.ComponentModel.DataAnnotations;
using CareerHub.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CareerHub.ViewModels
{
    /// <summary>Form Thông tin công ty (bảng CONGTY).</summary>
    public class CompanyProfileVm
    {
        public int MaCongTy { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên công ty")]
        [StringLength(100, ErrorMessage = "Tên công ty tối đa 100 ký tự")]
        [Display(Name = "Tên công ty")]
        public string TenCongTy { get; set; } = "";

        [StringLength(100, ErrorMessage = "Website tối đa 100 ký tự")]
        [Url(ErrorMessage = "Website không đúng định dạng (vd: https://congty.com)")]
        [Display(Name = "Website")]
        public string? Website { get; set; }

        [StringLength(150, ErrorMessage = "Địa chỉ tối đa 150 ký tự")]
        [Display(Name = "Địa chỉ")]
        public string? DiaChi { get; set; }

        [StringLength(50, ErrorMessage = "Quy mô tối đa 50 ký tự")]
        [Display(Name = "Quy mô")]
        public string? QuyMo { get; set; }

        [Display(Name = "Mô tả công ty")]
        public string? MoTa { get; set; }

        /// <summary>Đường dẫn logo hiện tại (chỉ để hiển thị).</summary>
        public string? Logo { get; set; }

        /// <summary>File logo mới upload (được kiểm tra thủ công trong controller).</summary>
        [Display(Name = "Logo công ty")]
        public IFormFile? LogoFile { get; set; }

        public bool? DaXacMinh { get; set; }
    }

    /// <summary>Form Thêm / Sửa tin tuyển dụng (bảng TINTUYENDUNG + KYNANGTIN).</summary>
    public class JobFormVm : IValidatableObject
    {
        public int MaTin { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tiêu đề tin")]
        [StringLength(150, ErrorMessage = "Tiêu đề tối đa 150 ký tự")]
        [Display(Name = "Tiêu đề")]
        public string TieuDe { get; set; } = "";

        [Required(ErrorMessage = "Vui lòng chọn ngành nghề")]
        [Display(Name = "Ngành nghề")]
        public string MaDanhMuc { get; set; } = "";

        [Required(ErrorMessage = "Vui lòng nhập mô tả công việc")]
        [Display(Name = "Mô tả công việc")]
        public string? MoTaCv { get; set; }

        [Display(Name = "Yêu cầu ứng viên")]
        public string? YeuCau { get; set; }

        [Display(Name = "Quyền lợi")]
        public string? QuyenLoi { get; set; }

        [StringLength(150, ErrorMessage = "Địa chỉ làm việc tối đa 150 ký tự")]
        [Display(Name = "Địa chỉ làm việc")]
        public string? DiaChiLamViec { get; set; }

        [Range(0, 1000000000000, ErrorMessage = "Lương không hợp lệ")]
        [Display(Name = "Lương từ")]
        public decimal? LuongTu { get; set; }

        [Range(0, 1000000000000, ErrorMessage = "Lương không hợp lệ")]
        [Display(Name = "Lương đến")]
        public decimal? LuongDen { get; set; }

        [StringLength(30)]
        [Display(Name = "Loại hình")]
        public string? LoaiHinh { get; set; }

        [StringLength(50, ErrorMessage = "Kinh nghiệm tối đa 50 ký tự")]
        [Display(Name = "Kinh nghiệm")]
        public string? KinhNghiem { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Hạn nộp hồ sơ")]
        public DateTime? HanNop { get; set; }

        /// <summary>Các kỹ năng được tích chọn (MaKyNang) -> bảng KYNANGTIN.</summary>
        public List<string> SelectedSkills { get; set; } = new();

        // Dữ liệu đổ ra form (không bind từ form)
        public List<SelectListItem> Categories { get; set; } = new();
        public List<Kynang> AllSkills { get; set; } = new();

        public static readonly string[] JobTypes = { "Full-time", "Part-time", "Remote", "Internship" };

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (LuongTu.HasValue && LuongDen.HasValue && LuongDen < LuongTu)
                yield return new ValidationResult("Lương đến phải lớn hơn hoặc bằng lương từ", new[] { nameof(LuongDen) });

            if (HanNop.HasValue && HanNop.Value.Date < DateTime.Today)
                yield return new ValidationResult("Hạn nộp hồ sơ không được ở quá khứ", new[] { nameof(HanNop) });
        }
    }

    /// <summary>Trạng thái hồ sơ ứng tuyển + màu nhãn (dùng chung cho server và AJAX).</summary>
    public static class ApplicationStatus
    {
        public const string Submitted = "Đã nộp";
        public const string Reviewed = "Đã duyệt";
        public const string Interview = "Mời phỏng vấn";
        public const string Hired = "Trúng tuyển";
        public const string Rejected = "Từ chối";

        public static readonly string[] All = { Submitted, Reviewed, Interview, Hired, Rejected };

        public static string BadgeClass(string? status) => status switch
        {
            Reviewed => "bg-info",
            Interview => "bg-warning text-dark",
            Hired => "bg-success",
            Rejected => "bg-danger",
            _ => "bg-secondary"
        };
    }
}
