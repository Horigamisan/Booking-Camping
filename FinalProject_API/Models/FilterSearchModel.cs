namespace FinalProject_API.Models
{
    public class FilterSearchModel
    {
        public DateTime? Ngaybatdau { get; set; }
        public DateTime? Ngayketthuc { get; set; }
        public int? Soluongnguoi { get; set; }
        public string? Giatien { get; set; }
        public string? Diemdanhgia { get; set; }
        public int? Loaivitri { get; set; }
        public int[]? Loaidichvu { get; set; }
    }
}
