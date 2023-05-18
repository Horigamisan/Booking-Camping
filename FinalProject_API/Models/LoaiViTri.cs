using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace FinalProject_API.Models
{
    public partial class LoaiViTri
    {
        public LoaiViTri()
        {
            ViTriCamTrais = new HashSet<ViTriCamTrai>();
        }

        public int LoaivitriId { get; set; }
        public string? Tenloaivitri { get; set; }
        public string? Mota { get; set; }
        public string? Meta { get; set; }
        [JsonIgnore]

        public virtual ICollection<ViTriCamTrai> ViTriCamTrais { get; set; }
    }
}
