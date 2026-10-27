using System;
using System.Collections.Generic;

namespace CareerHub.Models;

public partial class Danhmuc
{
    public string MaDanhMuc { get; set; } = null!;

    public string TenDanhMuc { get; set; } = null!;

    public string? MoTa { get; set; }

    public virtual ICollection<Tintuyendung> Tintuyendungs { get; set; } = new List<Tintuyendung>();
}
