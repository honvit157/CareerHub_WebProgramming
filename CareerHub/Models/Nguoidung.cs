using System;
using System.Collections.Generic;

namespace CareerHub.Models;

public partial class Nguoidung
{
    public int MaNguoiDung { get; set; }

    public string Email { get; set; } = null!;

    public string MatKhau { get; set; } = null!;

    public string HoTen { get; set; } = null!;

    public string? DienThoai { get; set; }

    public string? DiaChi { get; set; }

    public string? AnhDaiDien { get; set; }

    public int MaVaiTro { get; set; }

    public bool? TrangThai { get; set; }

    public DateTime? NgayTao { get; set; }

    public virtual ICollection<Congty> Congties { get; set; } = new List<Congty>();

    public virtual ICollection<Hosocv> Hosocvs { get; set; } = new List<Hosocv>();

    public virtual Vaitro MaVaiTroNavigation { get; set; } = null!;

    public virtual ICollection<Thongbao> Thongbaos { get; set; } = new List<Thongbao>();

    public virtual ICollection<Ungtuyen> Ungtuyens { get; set; } = new List<Ungtuyen>();

    public virtual ICollection<Vieclamluu> Vieclamluus { get; set; } = new List<Vieclamluu>();
}
