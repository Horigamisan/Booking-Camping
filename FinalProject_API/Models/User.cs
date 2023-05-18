using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace FinalProject_API.Models
{
    public partial class User
    {
        public User()
        {
            DatChos = new HashSet<DatCho>();
            HoaDons = new HashSet<HoaDon>();
        }

        public int UserId { get; set; }
        public string? Hoten { get; set; }
        public string? Email { get; set; }
        public string? Sdt { get; set; }
        public string? Phanquyen { get; set; }
        public string? Diachi { get; set; }
        public string? Password { get; set; }
        public bool? Confirmed { get; set; }
        public string? ConfirmCode { get; set; }

        [JsonIgnore]
        public virtual ICollection<DatCho> DatChos { get; set; }
        [JsonIgnore]
        public virtual ICollection<HoaDon> HoaDons { get; set; }
    }
}
