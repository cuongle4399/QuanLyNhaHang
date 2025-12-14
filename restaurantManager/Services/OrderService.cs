using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using restaurantManager.Models;

namespace restaurantManager.Services
{
    public class OrderService
    {
        // ✅ FIX: Thêm method lấy danh sách bàn từ DB
        public ObservableCollection<BanAn> LayDanhSachBanAnTuDb()
        {
            var query = @"SELECT MaBan, SoGhe, TrangThai FROM BanAn";
            var result = new ObservableCollection<BanAn>();

            try
            {
                var dt = DatabaseConnect.ExecuteTable(query);
                foreach (DataRow row in dt.Rows)
                {
                    result.Add(new BanAn
                    {
                        MaBan = row["MaBan"].ToString(),
                        SoGhe = (int)row["SoGhe"],
                        TrangThai = row["TrangThai"].ToString(),
                        IsSelected = false
                    });
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Lỗi khi lấy danh sách bàn: " + ex.Message);
            }

            return result;
        }

        // ✅ FIX: Thêm method lấy danh sách món ăn từ DB
        public ObservableCollection<MonAn> LayDanhSachMonAnTuBd()
        {
            var query = @"SELECT MaMonAn, TenMonAn, Gia, MoTa, HinhAnhURL, Loai FROM MonAn";
            var result = new ObservableCollection<MonAn>();

            try
            {
                var dt = DatabaseConnect.ExecuteTable(query);
                foreach (DataRow row in dt.Rows)
                {
                    result.Add(new MonAn
                    {
                        MaMonAn = (int)row["MaMonAn"],
                        TenMonAn = row["TenMonAn"].ToString(),
                        Gia = (decimal)row["Gia"],
                        MoTa = row["MoTa"].ToString(),
                        HinhAnhURL = row["HinhAnhURL"].ToString(),
                        Loai = (int)row["Loai"],
                        SoLuong = 1
                    });
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Lỗi khi lấy danh sách món ăn: " + ex.Message);
            }

            return result;
        }

        // ✅ FIX: Thêm method lấy danh sách món ăn có khuyến mãi
        public ObservableCollection<int> LayDanhSachMonAnKM()
        {
            var query = @"SELECT DISTINCT MaMonAn FROM MonAnApDungKM";
            var result = new ObservableCollection<int>();

            try
            {
                var dt = DatabaseConnect.ExecuteTable(query);
                foreach (DataRow row in dt.Rows)
                {
                    if (int.TryParse(row["MaMonAn"].ToString(), out int maMonAn))
                    {
                        result.Add(maMonAn);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Lỗi khi lấy danh sách khuyến mãi: " + ex.Message);
            }

            return result;
        }

        // ✅ FIX: Thêm method cập nhật trạng thái bàn
        public bool CapNhatTrangThaiBan(string maBan, string trangThai)
        {
            var query = @"UPDATE BanAn SET TrangThai = @TrangThai WHERE MaBan = @MaBan";
            var parameters = new[]
            {
                new SqlParameter("@MaBan", maBan),
                new SqlParameter("@TrangThai", trangThai)
            };

            return DatabaseConnect.ExecuteNonQuery(query, parameters);
        }

        // ✅ FIX: Thêm method chèn đơn hàng
        public int ChenDonHang(DonHang donHang)
        {
            var query = @"
                INSERT INTO DonHang (NgayDat, TongTien, MaNhanVien, TrangThai, HinhThucThanhToan, MaBan)
                VALUES (@NgayDat, @TongTien, @MaNhanVien, @TrangThai, @HinhThucThanhToan, @MaBan);
                SELECT SCOPE_IDENTITY();
            ";

            var parameters = new[]
            {
                new SqlParameter("@NgayDat", donHang.NgayDat),
                new SqlParameter("@TongTien", donHang.TongTien),
                new SqlParameter("@MaNhanVien", donHang.MaNhanVien),
                new SqlParameter("@TrangThai", donHang.TrangThai),
                new SqlParameter("@HinhThucThanhToan", donHang.HinhThucThanhToan ?? "TienMat"),
                new SqlParameter("@MaBan", donHang.MaBan ?? (object)DBNull.Value)
            };

            var result = DatabaseConnect.ExecuteScalar(query, parameters);
            return result != null && int.TryParse(result.ToString(), out int maDH) ? maDH : -1;
        }

        // ✅ FIX: Thêm method chèn chi tiết đơn hàng
        public bool ChenChiTietDonHang(ChiTietDonHang chiTiet)
        {
            var query = @"
                INSERT INTO ChiTietDonHang (MaDonHang, MaMonAn, SoLuong, MaKhuyenMai, TienGoc, SoTienGiam, ThanhToanCuoi)
                VALUES (@MaDonHang, @MaMonAn, @SoLuong, @MaKhuyenMai, @TienGoc, @SoTienGiam, @ThanhToanCuoi);
            ";

            var parameters = new[]
            {
                new SqlParameter("@MaDonHang", chiTiet.MaDonHang),
                new SqlParameter("@MaMonAn", chiTiet.MaMonAn),
                new SqlParameter("@SoLuong", chiTiet.SoLuong),
                new SqlParameter("@MaKhuyenMai", chiTiet.MaKhuyenMai ?? (object)DBNull.Value),
                new SqlParameter("@TienGoc", chiTiet.TienGoc),
                new SqlParameter("@SoTienGiam", chiTiet.SoTienGiam),
                new SqlParameter("@ThanhToanCuoi", chiTiet.ThanhToanCuoi)
            };

            return DatabaseConnect.ExecuteNonQuery(query, parameters);
        }

        // ✅ FIX: Thêm method lấy mã khuyến mãi của món ăn
        public int? LayMaKhuyenMaiCuaMonAn(int maMonAn)
        {
            var query = @"
                SELECT TOP 1 MaKhuyenMai 
                FROM MonAnApDungKM 
                WHERE MaMonAn = @MaMonAn
            ";

            var parameters = new[] { new SqlParameter("@MaMonAn", maMonAn) };
            var result = DatabaseConnect.ExecuteScalar(query, parameters);

            return result != null && int.TryParse(result.ToString(), out int maKM) ? maKM : (int?)null;
        }

        // ✅ FIX: Thêm method lấy thông tin khuyến mãi
        public KhuyenMai LayKhuyenMaiTheoMaKhuyenMai(int? maKhuyenMai)
        {
            if (maKhuyenMai == null) return null;

            var query = @"
                SELECT MaKhuyenMai, TenChuongTrinh, LoaiGiamGia, GiaTriGiam, NgayBatDau, NgayKetThuc, TrangThai
                FROM KhuyenMai
                WHERE MaKhuyenMai = @MaKhuyenMai
            ";

            var parameters = new[] { new SqlParameter("@MaKhuyenMai", maKhuyenMai) };
            var dt = DatabaseConnect.ExecuteTable(query, parameters);

            if (dt.Rows.Count == 0) return null;

            var row = dt.Rows[0];
            return new KhuyenMai
            {
                MaKhuyenMai = (int)row["MaKhuyenMai"],
                TenChuongTrinh = row["TenChuongTrinh"].ToString(),
                LoaiGiamGia = row["LoaiGiamGia"].ToString(),
                GiaTriGiam = (decimal)row["GiaTriGiam"],
                NgayBatDau = (DateTime)row["NgayBatDau"],
                NgayKetThuc = (DateTime)row["NgayKetThuc"],
                TrangThai = row["TrangThai"].ToString()
            };
        }
    }
}