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
        public string MaGiamGia { get; set; } // MaCode từ bảng KhuyenMai
        public string TenChuongTrinh { get; set; }
        public string LoaiGiamGia { get; set; } // "PhanTram" hoặc "TienCoDinh"
        public decimal GiaTriGiam { get; set; }
        public DateTime NgayBatDau { get; set; }
        public DateTime NgayKetThuc { get; set; }
        public string TrangThai { get; set; } // "HoatDong" hoặc "HetHan"

        // Tính toán từ dữ liệu
        public string GiaTri => LoaiGiamGia == "PhanTram"
            ? $"{GiaTriGiam}%"
            : $"{GiaTriGiam:N0}đ";

        // Dữ liệu món ăn áp dụng (danh sách tên món)
        public string MonAn { get; set; }

        public KhuyenMai() { }

        public KhuyenMai(
            int maKhuyenMai,
            string maCode,
            string tenChuongTrinh,
            string loaiGiamGia,
            decimal giaTriGiam,
            DateTime ngayBatDau,
            DateTime ngayKetThuc,
            string trangThai,
            string monAnList)
        {
            MaKhuyenMai = maKhuyenMai;
            MaGiamGia = maCode;
            TenChuongTrinh = tenChuongTrinh;
            LoaiGiamGia = loaiGiamGia;
            GiaTriGiam = giaTriGiam;
            NgayBatDau = ngayBatDau;
            NgayKetThuc = ngayKetThuc;
            TrangThai = trangThai;
            MonAn = monAnList;
        }
    }

    public class PromotionStats
    {
        public int Total { get; set; }
        public int Active { get; set; }
        public int Expired { get; set; }
        public decimal TotalSaved { get; set; } // Tổng tiền đã giảm (từ ChiTietDonHang)
    }
}