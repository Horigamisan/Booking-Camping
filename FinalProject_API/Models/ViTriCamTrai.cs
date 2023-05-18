using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace FinalProject_API.Models
{
    public partial class ViTriCamTrai
    {
        public ViTriCamTrai()
        {
            DatChos = new HashSet<DatCho>();
            GiaCas = new HashSet<GiaCa>();
            Dichvus = new HashSet<DichVuKemTheo>();
        }

        public int VitriId { get; set; }
        public string? Tenvitri { get; set; }
        public DateTime? Ngaythem { get; set; }
        public string? Mota { get; set; }
        public string? Motachitiet { get; set; }
        public decimal? Diemdanhgia { get; set; }
        public string? Hinhanh { get; set; }
        public int? Soluongnguoi { get; set; }
        public string? Trangthai { get; set; }
        public string? Meta { get; set; }
        public int? LoaivitriId { get; set; }

        [JsonIgnore]
        public virtual LoaiViTri? Loaivitri { get; set; }
        [JsonIgnore]
        public virtual ICollection<DatCho> DatChos { get; set; }
        public virtual ICollection<GiaCa> GiaCas { get; set; }
        [JsonIgnore]

        public virtual ICollection<DichVuKemTheo> Dichvus { get; set; }
    }
}
