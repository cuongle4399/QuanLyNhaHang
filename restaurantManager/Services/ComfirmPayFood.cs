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
                    TrangThai = trangThai
                });
            }

            return ListBanAn;
        }

        // Giữ nguyên các static methods khác...
        public ObservableCollection<ChiTiet> LayBangChiTiet(string maBan)
        {
            var result = new ObservableCollection<ChiTiet>();
            var sql = @"
                                SELECT ma.TenMonAn, ctdh.SoLuong, ctdh.TienGoc, ctdh.SoTienGiam, ctdh.ThanhToanCuoi, 
                                       ma.MaMonAn, ma.Gia, ctdh.MaKhuyenMai
                                FROM DonHang dh
                                INNER JOIN ChiTietDonHang ctdh ON dh.MaDonHang = ctdh.MaDonHang 
                                INNER JOIN MonAn ma ON ma.MaMonAn = ctdh.MaMonAn
                                WHERE dh.TrangThai = 'DangXuLy' AND dh.MaBan = @maBan";
            var dt = DatabaseConnect.ExecuteTable(sql, new SqlParameter("@maBan", maBan));
            foreach (DataRow r in dt.Rows)
            {
                int ma = Convert.ToInt32(r["MaMonAn"]);
                string ten = r["TenMonAn"].ToString();
                decimal gia = Convert.ToDecimal(r["Gia"]);
                int sl = Convert.ToInt32(r["SoLuong"]);
                decimal tienGoc = Convert.ToDecimal(r["TienGoc"]);
                decimal soTienGiam = Convert.ToDecimal(r["SoTienGiam"]);
                decimal thanh = Convert.ToDecimal(r["ThanhToanCuoi"]);
                int? makm = r["MaKhuyenMai"] != DBNull.Value ? (int?)Convert.ToInt32(r["MaKhuyenMai"]) : null;
                result.Add(new ChiTiet(ten, sl, tienGoc, soTienGiam, thanh, ma, gia, makm));

            }

            return result;
        }

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
                int MaDonHang = Convert.ToInt32(dt.Rows[0]["MaDonHang"]);
                DateTime NgayDat = Convert.ToDateTime(dt.Rows[0]["NgayDat"]);
                decimal TongTien = Convert.ToDecimal(dt.Rows[0]["TongTien"]);
                string MaNhanVien = dt.Rows[0]["MaNhanVien"].ToString().Trim();
                string TrangThai = dt.Rows[0]["TrangThai"].ToString().Trim();
                string MaBan = dt.Rows[0]["MaBan"].ToString().Trim();

                donHang = new DonHang(MaDonHang, NgayDat, TongTien, MaNhanVien, TrangThai, MaBan);
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

        public bool CapNhatTrangThaiDonHang(DonHang donHang, string trangThai, decimal TongTienPhaiThanhToan)
        {
            // Update DonHang set TrangThai = trangThai where MaDonHang = maDonHang

            string sql = @"UPDATE DonHang SET TrangThai=@tt, TongTien=@ttien WHERE MaDonHang=@mdh";

            return DatabaseConnect.ExecuteNonQuery(sql,
                                                        new SqlParameter("@tt", trangThai),
                                                        new SqlParameter("ttien", TongTienPhaiThanhToan),
                                                        new SqlParameter("mdh", donHang.MaDonHang));
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
