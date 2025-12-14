using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using restaurantManager.Models;

namespace restaurantManager.Services
{
    public class ComfirmPayFood : INotifyPropertyChanged
    {
        public ComfirmPayFood()
        {

        }

        // Hàm này thực hiện việc lấy dữ liệu bảng "BanAn" từ database
        public ObservableCollection<BanAn> LayDanhSachBanAnTuDb()
        {
            ObservableCollection<BanAn> ListBanAn = new ObservableCollection<BanAn>();

            DataTable dt = DatabaseConnect.ExecuteTable("SELECT MaBan, SoGhe, TrangThai FROM BanAn");

            foreach (DataRow dr in dt.Rows)
            {
                string maBan = dr["MaBan"].ToString().Trim();
                int soGhe = Convert.ToInt32(dr["SoGhe"]);
                string trangThai = dr["TrangThai"].ToString().Trim();

                ListBanAn.Add(new BanAn
                {
                    MaBan = maBan,
                    SoGhe = soGhe,
                    TrangThai = trangThai,
                    IsSelected = false
                });
            }

            return ListBanAn;
        }

        // ✅ FIX: Sửa LayBangChiTiet để không dùng constructor với tham số
        public ObservableCollection<ChiTiet> LayBangChiTiet(string maBan)
        {
            var result = new ObservableCollection<ChiTiet>();
            var sql = @"
                SELECT ma.TenMonAn, ctdh.SoLuong, ctdh.TienGoc, ctdh.SoTienGiam, ctdh.ThanhToanCuoi, 
                       ma.MaMonAn, ma.Gia, ctdh.MaKhuyenMai, ctdh.MaDonHang
                FROM DonHang dh
                INNER JOIN ChiTietDonHang ctdh ON dh.MaDonHang = ctdh.MaDonHang 
                INNER JOIN MonAn ma ON ma.MaMonAn = ctdh.MaMonAn
                WHERE dh.TrangThai = 'DangXuLy' AND dh.MaBan = @maBan";
            var dt = DatabaseConnect.ExecuteTable(sql, new SqlParameter("@maBan", maBan));
            foreach (DataRow r in dt.Rows)
            {
                // ✅ FIX: Sử dụng property assignment thay vì constructor
                var chiTiet = new ChiTiet
                {
                    MaDonHang = Convert.ToInt32(r["MaDonHang"]),
                    MaMonAn = Convert.ToInt32(r["MaMonAn"]),
                    TenMonAn = r["TenMonAn"].ToString(),
                    SoLuong = Convert.ToInt32(r["SoLuong"]),
                    TienGoc = Convert.ToDecimal(r["TienGoc"]),
                    SoTienGiam = Convert.ToDecimal(r["SoTienGiam"]),
                    ThanhToanCuoi = Convert.ToDecimal(r["ThanhToanCuoi"])
                };
                result.Add(chiTiet);
            }

            return result;
        }

        // ✅ FIX: Sửa LayDonHangMoiNhatTheoMaBan để không dùng constructor với tham số
        public DonHang LayDonHangMoiNhatTheoMaBan(string maBan)
        {
            DonHang donHang = null;

            string sql = @"
                SELECT TOP 1 dh.MaDonHang, dh.NgayDat, dh.TongTien, dh.MaNhanVien, dh.TrangThai, dh.MaBan
                FROM DonHang dh
                WHERE dh.MaBan=@mb
                ORDER BY NgayDat DESC
            ";
            var dt = DatabaseConnect.ExecuteTable(sql, new SqlParameter("@mb", maBan));

            if (dt.Rows.Count > 0)
            {
                // ✅ FIX: Sử dụng property assignment thay vì constructor
                donHang = new DonHang
                {
                    MaDonHang = Convert.ToInt32(dt.Rows[0]["MaDonHang"]),
                    NgayDat = Convert.ToDateTime(dt.Rows[0]["NgayDat"]),
                    TongTien = Convert.ToDecimal(dt.Rows[0]["TongTien"]),
                    MaNhanVien = dt.Rows[0]["MaNhanVien"].ToString().Trim(),
                    TrangThai = dt.Rows[0]["TrangThai"].ToString().Trim(),
                    MaBan = dt.Rows[0]["MaBan"].ToString().Trim()
                };
            }

            return donHang;
        }

        private DonHang _donHangCuaBan;
        public DonHang DonHangCuaBan
        {
            get => _donHangCuaBan;
            set
            {
                _donHangCuaBan = value;
                OnPropertyChanged();
            }
        }

        public bool CapNhatTrangThaiDonHang(DonHang donHang, string trangThai, decimal TongTienPhaiThanhToan, string phuongThuc)
        {
            // Update DonHang set TrangThai = trangThai where MaDonHang = maDonHang

            string sql = @"UPDATE DonHang SET TrangThai=@tt, TongTien=@ttien, HinhThucThanhToan=@httt WHERE MaDonHang=@mdh";

            return DatabaseConnect.ExecuteNonQuery(sql,
                                                        new SqlParameter("@tt", trangThai),
                                                        new SqlParameter("@ttien", TongTienPhaiThanhToan),
                                                        new SqlParameter("@mdh", donHang.MaDonHang),
                                                        new SqlParameter("@httt", phuongThuc));
        }

        public bool CapNhatTrangThaiBan(string maBan, string trangThai)
        {
            string sql = @"UPDATE BanAn SET TrangThai=@tt WHERE MaBan=@mb";

            return DatabaseConnect.ExecuteNonQuery(sql,
                                                        new SqlParameter("@tt", trangThai),
                                                        new SqlParameter("@mb", maBan));
        }


        // INotifyPropertyChanged implementation
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}