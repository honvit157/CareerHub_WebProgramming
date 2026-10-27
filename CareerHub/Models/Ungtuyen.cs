using System;
using System.Collections.Generic;

namespace CareerHub.Models;

public partial class Ungtuyen
{
    public int MaUngTuyen { get; set; }

    public int MaTin { get; set; }

    public int MaUngVien { get; set; }

    public int MaCv { get; set; }

    public string? ThuGioiThieu { get; set; }

    public string? TrangThai { get; set; }

    public DateTime? NgayNop { get; set; }

    public DateTime? NgayDuyet { get; set; }

    public virtual Hosocv MaCvNavigation { get; set; } = null!;

    public virtual Tintuyendung MaTinNavigation { get; set; } = null!;

    public virtual Nguoidung MaUngVienNavigation { get; set; } = null!;
}
