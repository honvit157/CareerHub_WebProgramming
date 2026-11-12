using CareerHub.Models;

namespace CareerHub.Helpers
{
    /// <summary>
    /// TV5 - Hàm dùng chung để tạo thông báo (bảng THONGBAO) cho một người dùng.
    /// Các thành viên khác gọi: NotificationHelper.Add(_db, userId, "Tiêu đề", "Nội dung");
    /// rồi tự gọi SaveChangesAsync() cùng lúc với thao tác chính của mình.
    /// </summary>
    public static class NotificationHelper
    {
        public static void Add(CareerHubContext db, int userId, string title, string content)
        {
            db.Thongbaos.Add(new Thongbao
            {
                MaNguoiDung = userId,
                TieuDe = Truncate(title, 150),
                NoiDung = Truncate(content, 500), // cột NoiDung tối đa 500 ký tự
                DaDoc = false,
                NgayTao = DateTime.Now
            });
        }

        private static string Truncate(string s, int max) => s.Length <= max ? s : s[..max];
    }
}