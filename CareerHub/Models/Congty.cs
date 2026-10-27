using System;
using System.Collections.Generic;

namespace CareerHub.Models;

public partial class Congty
{
    public int MaCongTy { get; set; }

    public int MaNhaTuyenDung { get; set; }

    public string TenCongTy { get; set; } = null!;

    public string? Logo { get; set; }

    public string? Website { get; set; }

    public string? DiaChi { get; set; }

    public string? QuyMo { get; set; }

    public string? MoTa { get; set; }

    public bool? DaXacMinh { get; set; }

    public DateTime? NgayTao { get; set; }

    public virtual Nguoidung MaNhaTuyenDungNavigation { get; set; } = null!;

    public virtual ICollection<Tintuyendung> Tintuyendungs { get; set; } = new List<Tintuyendung>();
}
