using System;
using System.Collections.Generic;

namespace CareerHub.Models;

public partial class Tintuyendung
{
    public int MaTin { get; set; }

    public int MaCongTy { get; set; }

    public string MaDanhMuc { get; set; } = null!;

    public string TieuDe { get; set; } = null!;

    public string? MoTaCv { get; set; }

    public string? YeuCau { get; set; }

    public string? QuyenLoi { get; set; }

    public string? DiaChiLamViec { get; set; }

    public decimal? LuongTu { get; set; }

    public decimal? LuongDen { get; set; }

    public string? LoaiHinh { get; set; }

    public string? KinhNghiem { get; set; }

    public DateTime? HanNop { get; set; }

    public string? TrangThai { get; set; }

    public int? SoLuotXem { get; set; }

    public DateTime? NgayDang { get; set; }

    public virtual Congty MaCongTyNavigation { get; set; } = null!;

    public virtual Danhmuc MaDanhMucNavigation { get; set; } = null!;

    public virtual ICollection<Ungtuyen> Ungtuyens { get; set; } = new List<Ungtuyen>();

    public virtual ICollection<Vieclamluu> Vieclamluus { get; set; } = new List<Vieclamluu>();

    public virtual ICollection<Kynang> MaKyNangs { get; set; } = new List<Kynang>();
}
