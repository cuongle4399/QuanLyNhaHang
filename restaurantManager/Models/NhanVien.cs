namespace restaurantManager.Models
{
    internal class NhanVien
    {
        public string MaNhanVien { get; set; }
        public string HoTen { get; set; }
        public DateTime NgaySinh { get; set; }
        public string SoDienThoai { get; set; }
        public string DiaChi { get; set; }
        public decimal Luong { get; set; }
        public string ChucVu { get; set; }
        public string TrangThai { get; set; }

        public NhanVien(string maNhanVien, string hoTen, DateTime ngaySinh,
                        string soDienThoai, string diaChi, decimal luong,
                        string chucVu, string trangThai)
        {
            MaNhanVien = maNhanVien;
            HoTen = hoTen;
            NgaySinh = ngaySinh;
            SoDienThoai = soDienThoai;
            DiaChi = diaChi;
            Luong = luong;
            ChucVu = chucVu;
            TrangThai = trangThai;
        }
    }

}
