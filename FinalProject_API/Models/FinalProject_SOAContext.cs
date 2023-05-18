using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace FinalProject_API.Models
{
    public partial class FinalProject_SOAContext : DbContext
    {
        public FinalProject_SOAContext()
        {
        }

        public FinalProject_SOAContext(DbContextOptions<FinalProject_SOAContext> options)
            : base(options)
        {
        }

        public virtual DbSet<DatCho> DatChos { get; set; } = null!;
        public virtual DbSet<DichVuKemTheo> DichVuKemTheos { get; set; } = null!;
        public virtual DbSet<GiaCa> GiaCas { get; set; } = null!;
        public virtual DbSet<HoaDon> HoaDons { get; set; } = null!;
        public virtual DbSet<LoaiViTri> LoaiViTris { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;
        public virtual DbSet<ViTriCamTrai> ViTriCamTrais { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=NGUYENPHUC\\TRONGPHUC;database=FinalProject_SOA;Trusted_Connection=True;MultipleActiveResultSets=True");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DatCho>(entity =>
            {
                entity.ToTable("DatCho");

                entity.Property(e => e.DatchoId).HasColumnName("datcho_id");

                entity.Property(e => e.Maxacnhan).HasColumnName("maxacnhan");

                entity.Property(e => e.Ngaybatdau)
                    .HasColumnType("smalldatetime")
                    .HasColumnName("ngaybatdau");

                entity.Property(e => e.Ngaydatcho)
                    .HasColumnType("smalldatetime")
                    .HasColumnName("ngaydatcho");

                entity.Property(e => e.Ngayketthuc)
                    .HasColumnType("smalldatetime")
                    .HasColumnName("ngayketthuc");

                entity.Property(e => e.Soluongnguoi).HasColumnName("soluongnguoi");

                entity.Property(e => e.Thoigianhuy)
                    .HasColumnType("smalldatetime")
                    .HasColumnName("thoigianhuy");

                entity.Property(e => e.Tongtien)
                    .HasColumnType("decimal(19, 4)")
                    .HasColumnName("tongtien");

                entity.Property(e => e.Trangthaidatcho)
                    .HasMaxLength(30)
                    .HasColumnName("trangthaidatcho");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.Property(e => e.VitriId).HasColumnName("vitri_id");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.DatChos)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK__DatCho__user_id__398D8EEE");

                entity.HasOne(d => d.Vitri)
                    .WithMany(p => p.DatChos)
                    .HasForeignKey(d => d.VitriId)
                    .HasConstraintName("FK__DatCho__vitri_id__3A81B327");
            });

            modelBuilder.Entity<DichVuKemTheo>(entity =>
            {
                entity.HasKey(e => e.DichvuId)
                    .HasName("PK__DichVuKe__1AE13C1A8285A59D");

                entity.ToTable("DichVuKemTheo");

                entity.Property(e => e.DichvuId).HasColumnName("dichvu_id");

                entity.Property(e => e.MoTa).HasColumnName("mo_ta");

                entity.Property(e => e.TenDichvu)
                    .HasMaxLength(50)
                    .HasColumnName("ten_dichvu");
            });

            modelBuilder.Entity<GiaCa>(entity =>
            {
                entity.HasKey(e => e.GiaId)
                    .HasName("PK__GiaCa__13068CBEBBA67A3C");

                entity.ToTable("GiaCa");

                entity.Property(e => e.GiaId).HasColumnName("gia_id");

                entity.Property(e => e.Giatien)
                    .HasColumnType("decimal(19, 4)")
                    .HasColumnName("giatien");

                entity.Property(e => e.Ngaybatdau)
                    .HasColumnType("smalldatetime")
                    .HasColumnName("ngaybatdau");

                entity.Property(e => e.Ngayketthuc)
                    .HasColumnType("smalldatetime")
                    .HasColumnName("ngayketthuc");

                entity.Property(e => e.PriceStripeId).HasColumnName("price_stripe_id");

                entity.Property(e => e.TiLeThue)
                    .HasColumnType("decimal(10, 2)")
                    .HasColumnName("ti_le_thue");

                entity.Property(e => e.VitriId).HasColumnName("vitri_id");

                entity.HasOne(d => d.Vitri)
                    .WithMany(p => p.GiaCas)
                    .HasForeignKey(d => d.VitriId)
                    .HasConstraintName("FK__GiaCa__vitri_id__36B12243");
            });

            modelBuilder.Entity<HoaDon>(entity =>
            {
                entity.ToTable("HoaDon");

                entity.Property(e => e.HoadonId).HasColumnName("hoadon_id");

                entity.Property(e => e.DatchoId).HasColumnName("datcho_id");

                entity.Property(e => e.GiaodichId)
                    .HasMaxLength(100)
                    .HasColumnName("giaodich_id");

                entity.Property(e => e.Loaitiente)
                    .HasMaxLength(10)
                    .HasColumnName("loaitiente");

                entity.Property(e => e.Phuongthuc)
                    .HasMaxLength(30)
                    .HasColumnName("phuongthuc");

                entity.Property(e => e.Sotienthanhtoan)
                    .HasColumnType("decimal(19, 4)")
                    .HasColumnName("sotienthanhtoan");

                entity.Property(e => e.Thoigiancapnhat)
                    .HasColumnType("smalldatetime")
                    .HasColumnName("thoigiancapnhat");

                entity.Property(e => e.Thoigiantao)
                    .HasColumnType("smalldatetime")
                    .HasColumnName("thoigiantao");

                entity.Property(e => e.Trangthai)
                    .HasMaxLength(50)
                    .HasColumnName("trangthai");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.HasOne(d => d.Datcho)
                    .WithMany(p => p.HoaDons)
                    .HasForeignKey(d => d.DatchoId)
                    .HasConstraintName("FK__HoaDon__datcho_i__3E52440B");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.HoaDons)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK__HoaDon__user_id__3D5E1FD2");
            });

            modelBuilder.Entity<LoaiViTri>(entity =>
            {
                entity.ToTable("LoaiViTri");

                entity.Property(e => e.LoaivitriId).HasColumnName("loaivitri_id");

                entity.Property(e => e.Meta).HasColumnName("meta");

                entity.Property(e => e.Mota).HasColumnName("mota");

                entity.Property(e => e.Tenloaivitri)
                    .HasMaxLength(50)
                    .HasColumnName("tenloaivitri");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.Property(e => e.ConfirmCode).HasMaxLength(50);

                entity.Property(e => e.Confirmed).HasDefaultValueSql("((0))");

                entity.Property(e => e.Diachi)
                    .HasMaxLength(40)
                    .HasColumnName("diachi")
                    .HasDefaultValueSql("(N'TP.HCM')");

                entity.Property(e => e.Email)
                    .HasMaxLength(30)
                    .HasColumnName("email")
                    .HasDefaultValueSql("(N'user@test.com')");

                entity.Property(e => e.Hoten)
                    .HasMaxLength(50)
                    .HasColumnName("hoten")
                    .HasDefaultValueSql("(N'user')");

                entity.Property(e => e.Password)
                    .HasMaxLength(25)
                    .HasColumnName("password")
                    .HasDefaultValueSql("(N'user1234')");

                entity.Property(e => e.Phanquyen)
                    .HasMaxLength(20)
                    .HasColumnName("phanquyen")
                    .HasDefaultValueSql("(N'user')");

                entity.Property(e => e.Sdt)
                    .HasMaxLength(11)
                    .IsUnicode(false)
                    .HasColumnName("sdt");
            });

            modelBuilder.Entity<ViTriCamTrai>(entity =>
            {
                entity.HasKey(e => e.VitriId)
                    .HasName("PK__ViTriCam__70A6A92F6031F1A8");

                entity.ToTable("ViTriCamTrai");

                entity.Property(e => e.VitriId).HasColumnName("vitri_id");

                entity.Property(e => e.Diemdanhgia)
                    .HasColumnType("decimal(10, 1)")
                    .HasColumnName("diemdanhgia");

                entity.Property(e => e.Hinhanh).HasColumnName("hinhanh");

                entity.Property(e => e.LoaivitriId).HasColumnName("loaivitri_id");

                entity.Property(e => e.Meta).HasColumnName("meta");

                entity.Property(e => e.Mota).HasColumnName("mota");

                entity.Property(e => e.Motachitiet).HasColumnName("motachitiet");

                entity.Property(e => e.Ngaythem)
                    .HasColumnType("smalldatetime")
                    .HasColumnName("ngaythem");

                entity.Property(e => e.Soluongnguoi).HasColumnName("soluongnguoi");

                entity.Property(e => e.Tenvitri)
                    .HasMaxLength(50)
                    .HasColumnName("tenvitri");

                entity.Property(e => e.Trangthai)
                    .HasMaxLength(40)
                    .HasColumnName("trangthai");

                entity.HasOne(d => d.Loaivitri)
                    .WithMany(p => p.ViTriCamTrais)
                    .HasForeignKey(d => d.LoaivitriId)
                    .HasConstraintName("FK__ViTriCamT__loaiv__2E1BDC42");

                entity.HasMany(d => d.Dichvus)
                    .WithMany(p => p.Vitris)
                    .UsingEntity<Dictionary<string, object>>(
                        "ViTriDichVu",
                        l => l.HasOne<DichVuKemTheo>().WithMany().HasForeignKey("DichvuId").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK__ViTriDich__dichv__33D4B598"),
                        r => r.HasOne<ViTriCamTrai>().WithMany().HasForeignKey("VitriId").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK__ViTriDich__vitri__32E0915F"),
                        j =>
                        {
                            j.HasKey("VitriId", "DichvuId").HasName("PK__ViTriDic__C108BAEEC88EB8FC");

                            j.ToTable("ViTriDichVu");

                            j.IndexerProperty<int>("VitriId").HasColumnName("vitri_id");

                            j.IndexerProperty<int>("DichvuId").HasColumnName("dichvu_id");
                        });
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
