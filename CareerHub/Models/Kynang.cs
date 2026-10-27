using System;
using System.Collections.Generic;

namespace CareerHub.Models;

public partial class Kynang
{
    public string MaKyNang { get; set; } = null!;

    public string TenKyNang { get; set; } = null!;

    public virtual ICollection<Tintuyendung> MaTins { get; set; } = new List<Tintuyendung>();
}
