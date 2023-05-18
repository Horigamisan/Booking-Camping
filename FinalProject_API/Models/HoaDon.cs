using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace FinalProject_API.Models
{
    public partial class HoaDon
    {
        public int HoadonId { get; set; }
        public string? Phuongthuc { get; set; }
        public DateTime? Thoigiantao { get; set; }
        public DateTime? Thoigiancapnhat { get; set; }
        public decimal? Sotienthanhtoan { get; set; }
        public string? Loaitiente { get; set; }
        public string? GiaodichId { get; set; }
        public string? Trangthai { get; set; }
        public int? DatchoId { get; set; }
        public int? UserId { get; set; }

        [JsonIgnore]
        public virtual DatCho? Datcho { get; set; }
        [JsonIgnore]
        public virtual User? User { get; set; }
    }
}
