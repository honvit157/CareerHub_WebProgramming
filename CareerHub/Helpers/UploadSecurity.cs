namespace CareerHub.Helpers
{
    /// <summary>
    /// TV2 - M6: Ràng buộc bảo mật upload file ảnh (Logo công ty).
    /// - Chỉ cho phép .png / .jpg / .jpeg
    /// - Kích thước tối đa 5MB
    /// - Kiểm tra "magic bytes" thực sự của file (chặn file độc hại đổi đuôi thành .jpg)
    /// - Tên file lưu trên server do server tự sinh (Guid), không dùng tên người dùng gửi lên
    /// </summary>
    public static class UploadSecurity
    {
        public const long MaxImageBytes = 5 * 1024 * 1024; // 5MB
        private static readonly string[] AllowedImageExtensions = { ".png", ".jpg", ".jpeg" };

        /// <summary>Trả về null nếu hợp lệ, ngược lại trả về thông báo lỗi.</summary>
        public static async Task<string?> ValidateImageAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return "Vui lòng chọn một file ảnh.";

            if (file.Length > MaxImageBytes)
                return "Dung lượng ảnh tối đa là 5MB.";

            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!AllowedImageExtensions.Contains(ext))
                return "Chỉ chấp nhận file ảnh .png, .jpg hoặc .jpeg.";

            // Đọc vài byte đầu để xác định định dạng thật của file
            var header = new byte[8];
            using (var stream = file.OpenReadStream())
            {
                var read = await stream.ReadAsync(header, 0, header.Length);
                if (read < 3) return "File ảnh không hợp lệ.";
            }

            bool isPng = header.Length >= 8 &&
                         header[0] == 0x89 && header[1] == 0x50 && header[2] == 0x4E && header[3] == 0x47 &&
                         header[4] == 0x0D && header[5] == 0x0A && header[6] == 0x1A && header[7] == 0x0A;
            bool isJpg = header[0] == 0xFF && header[1] == 0xD8 && header[2] == 0xFF;

            if (ext == ".png" && !isPng) return "Nội dung file không phải ảnh PNG hợp lệ.";
            if ((ext == ".jpg" || ext == ".jpeg") && !isJpg) return "Nội dung file không phải ảnh JPG hợp lệ.";

            return null;
        }

        /// <summary>Lưu ảnh vào wwwroot/{subFolder} và trả về đường dẫn web (vd: /uploads/logos/abc.png).</summary>
        public static async Task<string> SaveImageAsync(IFormFile file, string webRootPath, string subFolder)
        {
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            var folder = Path.Combine(webRootPath, subFolder.Replace('/', Path.DirectorySeparatorChar));
            Directory.CreateDirectory(folder);

            var fileName = $"{Guid.NewGuid():N}{ext}";
            var fullPath = Path.Combine(folder, fileName);
            using (var fs = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(fs);
            }
            return "/" + subFolder.Trim('/') + "/" + fileName;
        }

        /// <summary>Xoá file cũ nhưng CHỈ khi nó nằm trong thư mục uploads (chống path traversal).</summary>
        public static void DeleteUploadedFile(string? webPath, string webRootPath)
        {
            if (string.IsNullOrWhiteSpace(webPath)) return;

            var uploadsRoot = Path.GetFullPath(Path.Combine(webRootPath, "uploads")) + Path.DirectorySeparatorChar;
            var full = Path.GetFullPath(Path.Combine(webRootPath, webPath.TrimStart('/', '\\')));
            if (full.StartsWith(uploadsRoot, StringComparison.OrdinalIgnoreCase) && File.Exists(full))
            {
                try { File.Delete(full); } catch { /* bỏ qua lỗi xoá file */ }
            }
        }
    }
}
