using System;
using System.Collections.Generic;

namespace CareerHub.Models;

public partial class Thongbao
{
    public int MaThongBao { get; set; }

    public int MaNguoiDung { get; set; }

    public string TieuDe { get; set; } = null!;

    public string NoiDung { get; set; } = null!;

    public bool? DaDoc { get; set; }

    public DateTime? NgayTao { get; set; }

    public virtual Nguoidung MaNguoiDungNavigation { get; set; } = null!;
}
