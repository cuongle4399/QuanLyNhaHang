using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace restaurantManager.Models
{
    public class KhuyenMai
    {
        public int? MaKhuyenMai { get; set; }
        public string TenChuongTrinh { get; set; }
        public string MaCode { get; set; }
        public string LoaiGiamGia { get; set; }
        public decimal GiaTriGiam { get; set; }
        public DateTime NgayBatDau { get; set; }
        public DateTime NgayKetThuc { get; set; }
        public string TrangThai { get; set; }
        public int? SoLuotSuDungToiDa { get; set; }
        public int SoLuotDaSuDung { get; set; }
    }
}
