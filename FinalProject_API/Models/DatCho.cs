using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace FinalProject_API.Models
{
    public partial class DatCho
    {
        public DatCho()
        {
            HoaDons = new HashSet<HoaDon>();
        }

        public int DatchoId { get; set; }
        public DateTime? Ngaydatcho { get; set; }
        public DateTime? Ngaybatdau { get; set; }
        public DateTime? Ngayketthuc { get; set; }
        public DateTime? Thoigianhuy { get; set; }
        public int? Soluongnguoi { get; set; }
        public string? Trangthaidatcho { get; set; }
        public decimal? Tongtien { get; set; }
        public string? Maxacnhan { get; set; }
        public int? UserId { get; set; }
        public int? VitriId { get; set; }

        [JsonIgnore]
        public virtual User? User { get; set; }
        public virtual ViTriCamTrai? Vitri { get; set; }
        [JsonIgnore]
        public virtual ICollection<HoaDon> HoaDons { get; set; }
    }
}
