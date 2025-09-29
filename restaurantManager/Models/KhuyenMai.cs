using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace restaurantManager.Models
{
    public class KhuyenMai
    {
        public int MaKhuyenMai { get; set; }
        public string MaCode { get; set; } // Tên cột trong DB là MaCode
        public string TenChuongTrinh { get; set; }
        public string LoaiGiamGia { get; set; } // "PhanTram" hoặc "TienCoDinh"
        public decimal GiaTriGiam { get; set; }
        public DateTime NgayBatDau { get; set; }
        public DateTime NgayKetThuc { get; set; }
        public string TrangThai { get; set; } // "HoatDong" hoặc "HetHan"
        public int? SoLuotSuDungToiDa { get; set; } // Có thể null (không giới hạn)
        public int SoLuotDaSuDung { get; set; }

        // Các thuộc tính hiển thị (computed)
        public string MaGiamGia => MaCode;
        public string MonAn { get; set; } // Danh sách tên món ăn áp dụng, nối bằng dấu phẩy
        public string LoaiGiamGiaHiểnThị => LoaiGiamGia == "PhanTram" ? "Giảm %" : "Giảm tiền";
        public string GiaTri => LoaiGiamGia == "PhanTram" ? $"{GiaTriGiam}%" : $"{GiaTriGiam:N0}đ";
        public string NgayBatDauStr => NgayBatDau.ToString("dd/MM/yyyy HH:mm");
        public string NgayKetThucStr => NgayKetThuc.ToString("dd/MM/yyyy HH:mm");
        public string TrangThaiHiểnThị => TrangThai == "HoatDong" ? "Đang hoạt động" : "Hết hạn";
        public string ThaoTac => "Sửa | Xóa";

        public class PromotionStats
        {
            public int Total { get; set; }
            public int Active { get; set; }
            public int Expired { get; set; }
            public decimal TotalSaved { get; set; }
        }
    }
}
