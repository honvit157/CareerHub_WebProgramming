using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace CareerHub.Models;

public partial class CareerHubContext : DbContext
{
    public CareerHubContext()
    {
    }

    public CareerHubContext(DbContextOptions<CareerHubContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Congty> Congties { get; set; }

    public virtual DbSet<Danhmuc> Danhmucs { get; set; }

    public virtual DbSet<Hosocv> Hosocvs { get; set; }

    public virtual DbSet<Kynang> Kynangs { get; set; }

    public virtual DbSet<Nguoidung> Nguoidungs { get; set; }

    public virtual DbSet<Thongbao> Thongbaos { get; set; }

    public virtual DbSet<Tintuyendung> Tintuyendungs { get; set; }

    public virtual DbSet<Ungtuyen> Ungtuyens { get; set; }

    public virtual DbSet<Vaitro> Vaitros { get; set; }

    public virtual DbSet<Vieclamluu> Vieclamluus { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=DAVIHOAN\\SQLEXPRESS;Initial Catalog=CareerHub;Integrated Security=True;Connect Timeout=30;Encrypt=True;Trust Server Certificate=True;Application Intent=ReadWrite;Multi Subnet Failover=False");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Congty>(entity =>
        {
            entity.HasKey(e => e.MaCongTy).HasName("PK_CONGTY_MaCongTy");

            entity.ToTable("CONGTY");

            entity.Property(e => e.DaXacMinh).HasDefaultValue(false);
            entity.Property(e => e.DiaChi).HasMaxLength(150);
            entity.Property(e => e.Logo).HasMaxLength(255);
            entity.Property(e => e.NgayTao)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.QuyMo).HasMaxLength(50);
            entity.Property(e => e.TenCongTy).HasMaxLength(100);
            entity.Property(e => e.Website)
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.HasOne(d => d.MaNhaTuyenDungNavigation).WithMany(p => p.Congties)
                .HasForeignKey(d => d.MaNhaTuyenDung)
                .HasConstraintName("FK_CONGTY_MaNhaTuyenDung");
        });

        modelBuilder.Entity<Danhmuc>(entity =>
        {
            entity.HasKey(e => e.MaDanhMuc).HasName("PK_DANHMUC_MaDanhMuc");

            entity.ToTable("DANHMUC");

            entity.Property(e => e.MaDanhMuc)
                .HasMaxLength(4)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.MoTa).HasMaxLength(200);
            entity.Property(e => e.TenDanhMuc).HasMaxLength(50);
        });

        modelBuilder.Entity<Hosocv>(entity =>
        {
            entity.HasKey(e => e.MaCv).HasName("PK_HOSOCV_MaCV");

            entity.ToTable("HOSOCV");

            entity.Property(e => e.MaCv).HasColumnName("MaCV");
            entity.Property(e => e.DuongDanFile).HasMaxLength(255);
            entity.Property(e => e.MacDinh).HasDefaultValue(false);
            entity.Property(e => e.NgayCapNhat)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.TenCv)
                .HasMaxLength(100)
                .HasColumnName("TenCV");
            entity.Property(e => e.TieuSu).HasMaxLength(500);

            entity.HasOne(d => d.MaUngVienNavigation).WithMany(p => p.Hosocvs)
                .HasForeignKey(d => d.MaUngVien)
                .HasConstraintName("FK_HOSOCV_MaUngVien");
        });

        modelBuilder.Entity<Kynang>(entity =>
        {
            entity.HasKey(e => e.MaKyNang).HasName("PK_KYNANG_MaKyNang");

            entity.ToTable("KYNANG");

            entity.Property(e => e.MaKyNang)
                .HasMaxLength(4)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.TenKyNang).HasMaxLength(50);
        });

        modelBuilder.Entity<Nguoidung>(entity =>
        {
            entity.HasKey(e => e.MaNguoiDung).HasName("PK_NGUOIDUNG_MaNguoiDung");

            entity.ToTable("NGUOIDUNG");

            entity.Property(e => e.AnhDaiDien).HasMaxLength(255);
            entity.Property(e => e.DiaChi).HasMaxLength(100);
            entity.Property(e => e.DienThoai)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.HoTen).HasMaxLength(50);
            entity.Property(e => e.MatKhau)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.NgayTao)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.TrangThai).HasDefaultValue(true);

            entity.HasOne(d => d.MaVaiTroNavigation).WithMany(p => p.Nguoidungs)
                .HasForeignKey(d => d.MaVaiTro)
                .HasConstraintName("FK_NGUOIDUNG_MaVaiTro");
        });

        modelBuilder.Entity<Thongbao>(entity =>
        {
            entity.HasKey(e => e.MaThongBao).HasName("PK_THONGBAO_MaThongBao");

            entity.ToTable("THONGBAO");

            entity.Property(e => e.DaDoc).HasDefaultValue(false);
            entity.Property(e => e.NgayTao)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.NoiDung).HasMaxLength(500);
            entity.Property(e => e.TieuDe).HasMaxLength(150);

            entity.HasOne(d => d.MaNguoiDungNavigation).WithMany(p => p.Thongbaos)
                .HasForeignKey(d => d.MaNguoiDung)
                .HasConstraintName("FK_THONGBAO_MaNguoiDung");
        });

        modelBuilder.Entity<Tintuyendung>(entity =>
        {
            entity.HasKey(e => e.MaTin).HasName("PK_TINTUYENDUNG_MaTin");

            entity.ToTable("TINTUYENDUNG");

            entity.Property(e => e.DiaChiLamViec).HasMaxLength(150);
            entity.Property(e => e.HanNop).HasColumnType("datetime");
            entity.Property(e => e.KinhNghiem).HasMaxLength(50);
            entity.Property(e => e.LoaiHinh).HasMaxLength(30);
            entity.Property(e => e.LuongDen).HasColumnType("numeric(15, 2)");
            entity.Property(e => e.LuongTu).HasColumnType("numeric(15, 2)");
            entity.Property(e => e.MaDanhMuc)
                .HasMaxLength(4)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.MoTaCv).HasColumnName("MoTaCV");
            entity.Property(e => e.NgayDang)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.SoLuotXem).HasDefaultValue(0);
            entity.Property(e => e.TieuDe).HasMaxLength(150);
            entity.Property(e => e.TrangThai)
                .HasMaxLength(30)
                .HasDefaultValue("Chờ duyệt");

            entity.HasOne(d => d.MaCongTyNavigation).WithMany(p => p.Tintuyendungs)
                .HasForeignKey(d => d.MaCongTy)
                .HasConstraintName("FK_TINTUYENDUNG_MaCongTy");

            entity.HasOne(d => d.MaDanhMucNavigation).WithMany(p => p.Tintuyendungs)
                .HasForeignKey(d => d.MaDanhMuc)
                .HasConstraintName("FK_TINTUYENDUNG_MaDanhMuc");

            entity.HasMany(d => d.MaKyNangs).WithMany(p => p.MaTins)
                .UsingEntity<Dictionary<string, object>>(
                    "Kynangtin",
                    r => r.HasOne<Kynang>().WithMany()
                        .HasForeignKey("MaKyNang")
                        .HasConstraintName("FK_KYNANGTIN_MaKyNang"),
                    l => l.HasOne<Tintuyendung>().WithMany()
                        .HasForeignKey("MaTin")
                        .HasConstraintName("FK_KYNANGTIN_MaTin"),
                    j =>
                    {
                        j.HasKey("MaTin", "MaKyNang");
                        j.ToTable("KYNANGTIN");
                        j.IndexerProperty<string>("MaKyNang")
                            .HasMaxLength(4)
                            .IsUnicode(false)
                            .IsFixedLength();
                    });
        });

        modelBuilder.Entity<Ungtuyen>(entity =>
        {
            entity.HasKey(e => e.MaUngTuyen).HasName("PK_UNGTUYEN_MaUngTuyen");

            entity.ToTable("UNGTUYEN");

            entity.Property(e => e.MaCv).HasColumnName("MaCV");
            entity.Property(e => e.NgayDuyet).HasColumnType("datetime");
            entity.Property(e => e.NgayNop)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ThuGioiThieu).HasMaxLength(1000);
            entity.Property(e => e.TrangThai)
                .HasMaxLength(30)
                .HasDefaultValue("Đã nộp");

            entity.HasOne(d => d.MaCvNavigation).WithMany(p => p.Ungtuyens)
                .HasForeignKey(d => d.MaCv)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UNGTUYEN_MaCV");

            entity.HasOne(d => d.MaTinNavigation).WithMany(p => p.Ungtuyens)
                .HasForeignKey(d => d.MaTin)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UNGTUYEN_MaTin");

            entity.HasOne(d => d.MaUngVienNavigation).WithMany(p => p.Ungtuyens)
                .HasForeignKey(d => d.MaUngVien)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UNGTUYEN_MaUngVien");
        });

        modelBuilder.Entity<Vaitro>(entity =>
        {
            entity.HasKey(e => e.MaVaiTro).HasName("PK_VAITRO_MaVaiTro");

            entity.ToTable("VAITRO");

            entity.Property(e => e.TenVaiTro).HasMaxLength(50);
        });

        modelBuilder.Entity<Vieclamluu>(entity =>
        {
            entity.HasKey(e => new { e.MaUngVien, e.MaTin });

            entity.ToTable("VIECLAMLUU");

            entity.Property(e => e.NgayLuu)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.MaTinNavigation).WithMany(p => p.Vieclamluus)
                .HasForeignKey(d => d.MaTin)
                .HasConstraintName("FK_VIECLAMLUU_MaTin");

            entity.HasOne(d => d.MaUngVienNavigation).WithMany(p => p.Vieclamluus)
                .HasForeignKey(d => d.MaUngVien)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_VIECLAMLUU_MaUngVien");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
