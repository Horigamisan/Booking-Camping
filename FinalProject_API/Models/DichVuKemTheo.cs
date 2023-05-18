using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace FinalProject_API.Models
{
    public partial class DichVuKemTheo
    {
        public DichVuKemTheo()
        {
            Vitris = new HashSet<ViTriCamTrai>();
        }

        public int DichvuId { get; set; }
        public string? TenDichvu { get; set; }
        public string? MoTa { get; set; }
        [JsonIgnore]

        public virtual ICollection<ViTriCamTrai> Vitris { get; set; }
    }
}
