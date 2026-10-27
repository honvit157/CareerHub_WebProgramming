using System;
using System.Collections.Generic;

namespace CareerHub.Models;

public partial class Vieclamluu
{
    public int MaUngVien { get; set; }

    public int MaTin { get; set; }

    public DateTime? NgayLuu { get; set; }

    public virtual Tintuyendung MaTinNavigation { get; set; } = null!;

    public virtual Nguoidung MaUngVienNavigation { get; set; } = null!;
}
