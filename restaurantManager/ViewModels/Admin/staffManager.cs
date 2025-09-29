using System;
using System.Collections.ObjectModel;
using System.Data;
using restaurantManager.Models;
using restaurantManager.Services;
using System.Data.SqlClient;

namespace restaurantManager.ViewModels.Admin
{
    public class StaffManager
    {
        public ObservableCollection<NhanVien> DanhSachNhanVien { get; set; }

        public StaffManager()
        {
            DanhSachNhanVien = new ObservableCollection<NhanVien>();
            LoadDataNhanVien();
        }

        public void LoadDataNhanVien()
        {
            DataTable data = DatabaseConnect.ExecuteTable(
                "SELECT nv.*, nd.MatKhau, nd.VaiTro " +
                "FROM NhanVien nv " +
                "JOIN NguoiDung nd ON nv.MaNhanVien = nd.MaNhanVien " +
                "WHERE nd.VaiTro = 0"
            );

            DanhSachNhanVien.Clear();

            foreach (DataRow row in data.Rows)
            {
                var nv = new NhanVien
                {
                    MaNhanVien = row["MaNhanVien"]?.ToString() ?? "",
                    HoTen = row["HoTen"]?.ToString() ?? "",
                    SoDienThoai = row["SoDienThoai"]?.ToString() ?? "",
                    DiaChi = row["DiaChi"]?.ToString() ?? "",
                    ChucVu = row["ChucVu"]?.ToString() ?? "",
                    TrangThai = row["TrangThai"]?.ToString() ?? ""
                };

                if (DateTime.TryParse(row["NgaySinh"]?.ToString(), out DateTime ngaySinh))
                    nv.NgaySinh = ngaySinh;

                if (decimal.TryParse(row["Luong"]?.ToString(), out decimal luong))
                    nv.Luong = luong;

                DanhSachNhanVien.Add(nv);
            }
        }
        public static void InsertFullData(NhanVien nv, string matKhau, int vaiTro = 0)
        {
            string queryNguoiDung = @"INSERT INTO NguoiDung (MaNhanVien, MatKhau, VaiTro)
                                      VALUES (@maNv, @matKhau, @vaiTro)";

            SqlParameter[] parametersNguoiDung = new SqlParameter[]
            {
                new SqlParameter("@maNv", nv.MaNhanVien),
                new SqlParameter("@matKhau", matKhau),
                new SqlParameter("@vaiTro", vaiTro)
            };

            DatabaseConnect.ExecuteNonQuery(queryNguoiDung, parametersNguoiDung);

            string queryNhanVien = @"INSERT INTO NhanVien (MaNhanVien, HoTen, NgaySinh, SoDienThoai, DiaChi, Luong, ChucVu, TrangThai)
                                     VALUES (@maNv, @hoTen, @ngaySinh, @soDienThoai, @diaChi, @luong, @chucVu, @trangThai)";

            SqlParameter[] parametersNhanVien = new SqlParameter[]
            {
                new SqlParameter("@maNv", nv.MaNhanVien),
                new SqlParameter("@hoTen", nv.HoTen ?? (object)DBNull.Value),
                new SqlParameter("@ngaySinh", nv.NgaySinh == default ? (object)DBNull.Value : nv.NgaySinh),
                new SqlParameter("@soDienThoai", nv.SoDienThoai ?? (object)DBNull.Value),
                new SqlParameter("@diaChi", nv.DiaChi ?? (object)DBNull.Value),
                new SqlParameter("@luong", nv.Luong),
                new SqlParameter("@chucVu", nv.ChucVu ?? (object)DBNull.Value),
                new SqlParameter("@trangThai", nv.TrangThai ?? (object)DBNull.Value)
            };

            DatabaseConnect.ExecuteNonQuery(queryNhanVien, parametersNhanVien);
        }

        public static void UpdateData(NhanVien nv)
        {
            string query = @"UPDATE NhanVien 
                             SET HoTen = @hoTen, 
                                 NgaySinh = @ngaySinh,
                                 SoDienThoai = @soDienThoai,
                                 DiaChi = @diaChi, 
                                 Luong = @luong, 
                                 ChucVu = @chucVu, 
                                 TrangThai = @trangThai
                             WHERE MaNhanVien = @maNv";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@maNv", nv.MaNhanVien),
                new SqlParameter("@hoTen", nv.HoTen ?? (object)DBNull.Value),
                new SqlParameter("@ngaySinh", nv.NgaySinh == default ? (object)DBNull.Value : nv.NgaySinh),
                new SqlParameter("@soDienThoai", nv.SoDienThoai ?? (object)DBNull.Value),
                new SqlParameter("@diaChi", nv.DiaChi ?? (object)DBNull.Value),
                new SqlParameter("@luong", nv.Luong),
                new SqlParameter("@chucVu", nv.ChucVu ?? (object)DBNull.Value),
                new SqlParameter("@trangThai", nv.TrangThai ?? (object)DBNull.Value)
            };

            DatabaseConnect.ExecuteNonQuery(query, parameters);
        }

        public static void DeleteData(string maNhanVien)
        {
            string query = "DELETE FROM NguoiDung WHERE MaNhanVien = @maNv";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@maNv", maNhanVien)
            };

            DatabaseConnect.ExecuteNonQuery(query, parameters);
        }
    }
}
