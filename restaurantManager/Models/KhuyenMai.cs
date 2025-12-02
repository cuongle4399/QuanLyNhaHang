using System;

namespace restaurantManager.Models
{
    public class KhuyenMai
    {
        public int MaKhuyenMai { get; set; }
        public string MaCode { get; set; }   // SỬA TÊN CHO TRÙNG DB
        public string TenChuongTrinh { get; set; }
        public string LoaiGiamGia { get; set; }
        public decimal GiaTriGiam { get; set; }
        public DateTime NgayBatDau { get; set; }
        public DateTime NgayKetThuc { get; set; }
        public string TrangThai { get; set; }

        // BỔ SUNG cho đúng DB
        public int? SoLuotSuDungToiDa { get; set; }
        public int SoLuotDaSuDung { get; set; }

        // Tính toán hiển thị
        public string GiaTri => LoaiGiamGia == "PhanTram"
            ? $"{GiaTriGiam}%"
            : $"{GiaTriGiam:N0}đ";

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
            MaCode = maCode;
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
        public decimal TotalSaved { get; set; }
    }
}
