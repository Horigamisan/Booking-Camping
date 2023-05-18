using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace FinalProject_API.Models
{
    public partial class GiaCa
    {
        public int GiaId { get; set; }
        public DateTime? Ngaybatdau { get; set; }
        public DateTime? Ngayketthuc { get; set; }
        public decimal? Giatien { get; set; }
        public decimal? TiLeThue { get; set; }
        public string? PriceStripeId { get; set; }
        public int? VitriId { get; set; }
        [JsonIgnore]

        public virtual ViTriCamTrai? Vitri { get; set; }
    }
}
