using System;
using System.Collections.Generic;

namespace CareerHub.Models;

public partial class Hosocv
{
    public int MaCv { get; set; }

    public int MaUngVien { get; set; }

    public string TenCv { get; set; } = null!;

    public string DuongDanFile { get; set; } = null!;

    public string? TieuSu { get; set; }

    public bool? MacDinh { get; set; }

    public DateTime? NgayCapNhat { get; set; }

    public virtual Nguoidung MaUngVienNavigation { get; set; } = null!;

    public virtual ICollection<Ungtuyen> Ungtuyens { get; set; } = new List<Ungtuyen>();
}
