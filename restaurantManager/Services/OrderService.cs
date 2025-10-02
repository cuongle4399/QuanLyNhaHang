using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using restaurantManager.Models;

namespace restaurantManager.Services
{
    public class OrderService
    {
        // THÊM STATIC EVENT ĐỒNG BỘ TRẠNG THÁI BÀN
        public static event Action<string, string> OnTrangThaiBanThayDoi;

        // Method cập nhật trạng thái bàn - THÊM KÍCH HOẠT EVENT
        public void CapNhatTrangThaiBan(string maBan, string trangThaiMoi)
        {
            // Code hiện tại của bạn...
            string sql = "UPDATE BanAn SET TrangThai = @trangThai WHERE MaBan = @maBan";
            DatabaseConnect.ExecuteNonQuery(sql,
                new SqlParameter("@trangThai", trangThaiMoi),
                new SqlParameter("@maBan", maBan));

            // KÍCH HOẠT EVENT - THÔNG BÁO CHO CÁC MÀN HÌNH KHÁC
            OnTrangThaiBanThayDoi?.Invoke(maBan, trangThaiMoi);
        }

        public  int LayMaKhuyenMaiCuaMonAn(int maMonAn)
        {
            int maKhuyenMai;

            string sql = @"SELECT MaKhuyenMai FROM MonAnApDungKM WHERE MaMonAn = @maMonAn";

            maKhuyenMai = Convert.ToInt32(DatabaseConnect.ExecuteScalar(sql, new SqlParameter("@maMonAn", maMonAn)));

            return maKhuyenMai;
        }

        public  KhuyenMai LayKhuyenMaiTheoMaKhuyenMai(int? maKM)
        {
            KhuyenMai result = null;

            string sql = "SELECT * FROM KhuyenMai WHERE MaKhuyenMai = @maKhuyenMai";
            SqlParameter[] parameters = {
                new SqlParameter("@maKhuyenMai", maKM)
            };

            DataTable dt = DatabaseConnect.ExecuteTable(sql, parameters);

            if (dt != null && dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];
                result = new KhuyenMai();

                result.MaKhuyenMai = maKM;
                result.TenChuongTrinh = row["TenChuongTrinh"].ToString().Trim();
                result.MaCode = row["MaCode"].ToString().Trim();
                result.LoaiGiamGia = row["LoaiGiamGia"].ToString().Trim();
                result.GiaTriGiam = Convert.ToDecimal(row["GiaTriGiam"]);
                result.NgayBatDau = Convert.ToDateTime(row["NgayBatDau"]);
                result.NgayKetThuc = Convert.ToDateTime(row["NgayKetThuc"]);
                result.TrangThai = row["TrangThai"].ToString().Trim();
                result.SoLuotSuDungToiDa = Convert.ToInt32(row["SoLuotSuDungToiDa"]);
                result.SoLuotDaSuDung = Convert.ToInt32(row["SoLuotDaSuDung"]);
            }

            return result;
        }

        public  ObservableCollection<int> LayDanhSachMonAnKM()
        {
            ObservableCollection<int> DanhSachMaMonAnCoKhuyenMai = new ObservableCollection<int>();

            string sql = @"SELECT MaMonAn FROM MonAnApDungKM";
            DataTable dt = DatabaseConnect.ExecuteTable(sql);

            foreach (DataRow dr in dt.Rows)
            {
                DanhSachMaMonAnCoKhuyenMai.Add(Convert.ToInt32(dr["MaMonAn"]));
            }

            return DanhSachMaMonAnCoKhuyenMai;
        }

        // Hàm này thực hiện việc lấy dữ liệu bảng "BanAn" từ database
        public  ObservableCollection<BanAn> LayDanhSachBanAnTuDb()
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

        // Load danh sách món ăn từ database
        public  ObservableCollection<MonAn> LayDanhSachMonAnTuBd()
        {
            ObservableCollection<MonAn> ListBanAn = new ObservableCollection<MonAn>();

            string sql = @"SELECT 
                                MaMonAn, 
                                TenMonAn, 
                                Gia, 
                                MoTa,
                                HinhAnhURL, 
                                Loai
                            FROM MonAn";

            DataTable dt = DatabaseConnect.ExecuteTable(sql);

            foreach (DataRow dr in dt.Rows)
            {
                int maMonAn = Convert.ToInt32(dr["MaMonAn"]);
                string tenMonAn = dr["TenMonAn"].ToString().Trim();
                float gia = float.Parse(dr["Gia"].ToString().Trim());
                string moTa = dr["MoTa"].ToString().Trim();
                string hinhAnhURL = dr["HinhAnhURL"].ToString().Trim(); ;
                if (hinhAnhURL[0] != '/') hinhAnhURL = '/' + hinhAnhURL;
                int loai = Convert.ToInt32(dr["Loai"]);

                ListBanAn.Add(new MonAn
                {
                    MaMonAn = maMonAn,
                    TenMonAn = tenMonAn,
                    Gia = gia,
                    MoTa = moTa,
                    HinhAnhURL = hinhAnhURL,
                    Loai = loai
                });
            }

            return ListBanAn;
        }

  

        public  int ChenDonHang(DonHang dh)
        {
            string sql = @"
                            INSERT INTO DonHang (NgayDat, 
                                                TongTien, 
                                                MaNhanVien, 
                                                TrangThai, 
                                                MaBan)
                            OUTPUT INSERTED.MaDonHang
                            VALUES (@NgayDat, @TongTien, @MaNhanVien, @TrangThai, @MaBan)";

            object result = DatabaseConnect.ExecuteScalar(sql,
                new SqlParameter("@NgayDat", dh.NgayDat),
                new SqlParameter("@TongTien", dh.TongTien),
                new SqlParameter("@MaNhanVien", dh.MaNhanVien),
                new SqlParameter("@TrangThai", dh.TrangThai),
                new SqlParameter("@MaBan", dh.MaBan));
            return result != null ? Convert.ToInt32(result) : -1;
        }

        public  bool ChenChiTietDonHang(ChiTietDonHang ct)
        {
            string sql = @"INSERT INTO ChiTietDonHang 
                                            (MaDonHang, 
                                            MaMonAn, 
                                            SoLuong, 
                                            TienGoc, 
                                            SoTienGiam, 
                                            ThanhToanCuoi, 
                                            MaKhuyenMai)
                            VALUES (@MaDonHang, @MaMonAn, @SoLuong, @TienGoc, 
                                    @SoTienGiam, @ThanhToanCuoi, @MaKhuyenMai)";

            return DatabaseConnect.ExecuteNonQuery(sql,
                new SqlParameter("@MaDonHang", ct.MaDonHang),
                new SqlParameter("@MaMonAn", ct.MaMonAn),
                new SqlParameter("@SoLuong", ct.SoLuong),
                new SqlParameter("@TienGoc", ct.TienGoc),
                new SqlParameter("@SoTienGiam", ct.SoTienGiam),
                new SqlParameter("@ThanhToanCuoi", ct.ThanhToanCuoi),
                new SqlParameter("@MaKhuyenMai", (object?)ct.MaKhuyenMai ?? DBNull.Value));
        }

        public  DonHang LayDonHangMoiNhatCuaBan(string maBan)
        {
            var dt = DatabaseConnect.ExecuteTable(
                        @"SELECT 
                            TOP 1 * 
                        FROM DonHang 
                        WHERE MaBan=@ma AND TrangThai='DangXuLy' ORDER BY NgayDat DESC",
                new SqlParameter("@ma", maBan));
            
            if (dt.Rows.Count == 0) return null;
            var row = dt.Rows[0];
            return new DonHang
            {
                MaDonHang = Convert.ToInt32(row["MaDonHang"]),
                NgayDat = Convert.ToDateTime(row["NgayDat"]),
                TongTien = Convert.ToDecimal(row["TongTien"]),
                MaNhanVien = row["MaNhanVien"].ToString(),
                TrangThai = row["TrangThai"].ToString(),
                MaBan = row["MaBan"].ToString()
            };
        }

        public  List<(int MaMonAn, string TenMonAn, decimal Gia, int SoLuong, decimal TienGoc, decimal SoTienGiam, decimal ThanhToanCuoi, int? MaKhuyenMai)> GetChiTietByOrder(int maDonHang)
        {
            var result = new List<(int, string, decimal, int, decimal, decimal, decimal, int?)>();
            var sql = @"SELECT ct.MaMonAn, m.TenMonAn, m.Gia, ct.SoLuong, ct.TienGoc, ct.SoTienGiam, ct.ThanhToanCuoi, ct.MaKhuyenMai
                FROM ChiTietDonHang ct
                LEFT JOIN MonAn m ON ct.MaMonAn = m.MaMonAn
                WHERE ct.MaDonHang = @ma";
            var dt = DatabaseConnect.ExecuteTable(sql, new SqlParameter("@ma", maDonHang));
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
                result.Add((ma, ten, gia, sl, tienGoc, soTienGiam, thanh, makm));
            }
            return result;
        }

        public  List<KhuyenMai> GetActivePromotions()
        {
            var promotions = new List<KhuyenMai>();
            var dt = DatabaseConnect.ExecuteTable(
                "SELECT * FROM KhuyenMai WHERE TrangThai = 'HoatDong' AND NgayBatDau <= GETDATE() AND NgayKetThuc >= GETDATE()");
            foreach (DataRow row in dt.Rows)
            {
                promotions.Add(new KhuyenMai
                {
                    MaKhuyenMai = Convert.ToInt32(row["MaKhuyenMai"]),
                    TenChuongTrinh = row["TenChuongTrinh"].ToString(),
                    MaCode = row["MaCode"].ToString(),
                    LoaiGiamGia = row["LoaiGiamGia"].ToString(),
                    GiaTriGiam = decimal.Parse(row["GiaTriGiam"].ToString().Trim()),
                    NgayBatDau = Convert.ToDateTime(row["NgayBatDau"]),
                    NgayKetThuc = Convert.ToDateTime(row["NgayKetThuc"]),
                    TrangThai = row["TrangThai"].ToString(),
                    SoLuotSuDungToiDa = row["SoLuotSuDungToiDa"] != DBNull.Value ? (int?)Convert.ToInt32(row["SoLuotSuDungToiDa"]) : null,
                    SoLuotDaSuDung = Convert.ToInt32(row["SoLuotDaSuDung"])
                });
            }
            return promotions;
        }

        public  bool IsDishInPromotion(int maKhuyenMai, int maMonAn)
        {
            var result = DatabaseConnect.ExecuteScalar(
                "SELECT COUNT(*) FROM MonAnApDungKM WHERE MaKhuyenMai = @maKM AND MaMonAn = @maMon",
                new SqlParameter("@maKM", maKhuyenMai),
                new SqlParameter("@maMon", maMonAn));
            return Convert.ToInt32(result) > 0;
        }

        public  void IncrementPromotionUsage(int maKhuyenMai)
        {
            DatabaseConnect.ExecuteNonQuery(
                "UPDATE KhuyenMai SET SoLuotDaSuDung = SoLuotDaSuDung + 1 WHERE MaKhuyenMai = @maKM",
                new SqlParameter("@maKM", maKhuyenMai));
        }

        public  void PayOrder(int maDonHang, string maBan, List<ChiTietDonHang> details, string paymentMethod)
        {
            // vì DatabaseConnect không hỗ trợ transaction sẵn
            // ta vẫn phải dùng thủ công SqlConnection + SqlTransaction
            using (var conn = new SqlConnection(@"Server=LAPTOP-JLIOMIRV\ADMIN;Database=QuanLyNhaHang;Trusted_Connection=True;"))
            {
                conn.Open();
                using (var tran = conn.BeginTransaction())
                {
                    try
                    {
                        foreach (var it in details)
                        {
                            string updCt = @"UPDATE ChiTietDonHang
                                     SET SoTienGiam = @soTienGiam, ThanhToanCuoi = @thanhToan
                                     WHERE MaDonHang = @maDH AND MaMonAn = @maMon";
                            using var cmdUpdCt = new SqlCommand(updCt, conn, tran);
                            cmdUpdCt.Parameters.AddWithValue("@soTienGiam", it.SoTienGiam);
                            cmdUpdCt.Parameters.AddWithValue("@thanhToan", it.ThanhToanCuoi);
                            cmdUpdCt.Parameters.AddWithValue("@maDH", maDonHang);
                            cmdUpdCt.Parameters.AddWithValue("@maMon", it.MaMonAn);
                            cmdUpdCt.ExecuteNonQuery();
                        }

                        string updDh = "UPDATE DonHang SET TrangThai = 'DaHoanThanh', HinhThucThanhToan = @paymentMethod WHERE MaDonHang = @ma";
                        using var cmdUpdDh = new SqlCommand(updDh, conn, tran);
                        cmdUpdDh.Parameters.AddWithValue("@paymentMethod", paymentMethod);
                        cmdUpdDh.Parameters.AddWithValue("@ma", maDonHang);
                        cmdUpdDh.ExecuteNonQuery();

                        string updBan = "UPDATE BanAn SET TrangThai = 'Trống' WHERE MaBan = @maBan";
                        using var cmdUpdBan = new SqlCommand(updBan, conn, tran);
                        cmdUpdBan.Parameters.AddWithValue("@maBan", maBan);
                        cmdUpdBan.ExecuteNonQuery();

                        tran.Commit();
                    }
                    catch
                    {
                        tran.Rollback();
                        throw;
                    }
                }
            }
        }

        public  List<(int MaMonAn, string TenMonAn, decimal Gia, int SoLuong, decimal TienGoc, decimal SoTienGiam, decimal ThanhToanCuoi, int? MaKhuyenMai)> GetChiTietByTable(string maBan)
        {
            var result = new List<(int, string, decimal, int, decimal, decimal, decimal, int?)>();
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
                result.Add((ma, ten, gia, sl, tienGoc, soTienGiam, thanh, makm));
            }
            return result;
        }
    }
}
