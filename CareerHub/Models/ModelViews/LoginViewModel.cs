using System.ComponentModel.DataAnnotations;

namespace CareerHub.Models.ModelViews
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập Email")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Vui lòng nhập Mật khẩu")]
        [DataType(DataType.Password)]
        public string MatKhau { get; set; } = null!;

        public bool GhiNho { get; set; } // Checkbox "Ghi nhớ đăng nhập"
    }
}