using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;

namespace restaurantManager.Services
{
    /// <summary>
    /// Class tiện ích kết nối và thao tác với SQL Server bằng ADO.NET
    /// </summary>
    public static class DatabaseConnect
    {
        // TODO: Thay bằng thông tin SQL Server của bạn
        // - Nếu dùng SQL Authentication: "Server=.;Database=QuanLyNhaHang;User Id=sa;Password=123;"
        // - Nếu dùng Windows Authentication: "Server=.;Database=QuanLyNhaHang;Trusted_Connection=True;"
        private static string connectionString = @"Server=CUONG-LE;Database=QuanLyNhaHang;Trusted_Connection=True;";

        /// <summary>
        /// Thực hiện INSERT, UPDATE, DELETE
        /// </summary>
        /// <param name="query">Câu lệnh SQL</param>
        /// <param name="parameters">Danh sách tham số (SqlParameter)</param>
        /// <returns>true nếu có dòng bị ảnh hưởng</returns>
        /// <example>
        /// Ví dụ dùng:
        /// DatabaseConnect.ExecuteNonQuery(
        ///     "INSERT INTO NguoiDung (TaiKhoan, MatKhau, VaiTro) VALUES (@tk, @mk, @vt)",
        ///     new SqlParameter("@tk", "user1"),
        ///     new SqlParameter("@mk", "123"),
        ///     new SqlParameter("@vt", 0)
        /// );
        /// </example>
        public static bool ExecuteNonQuery(string query, params SqlParameter[] parameters)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        if (parameters != null)
                            cmd.Parameters.AddRange(parameters);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi Database: " + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Lấy 1 giá trị đơn (ví dụ SELECT COUNT, MAX, MIN, ...)
        /// </summary>
        /// <param name="query">Câu lệnh SQL</param>
        /// <param name="parameters">Danh sách tham số</param>
        /// <returns>object (cần ép kiểu)</returns>
        /// <example>
        /// Ví dụ dùng:
        /// int count = Convert.ToInt32(DatabaseConnect.ExecuteScalar(
        ///     "SELECT COUNT(*) FROM NguoiDung WHERE VaiTro = @vt",
        ///     new SqlParameter("@vt", 1)
        /// ));
        /// </example>
        public static object ExecuteScalar(string query, params SqlParameter[] parameters)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        if (parameters != null)
                            cmd.Parameters.AddRange(parameters);

                        return cmd.ExecuteScalar();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi Database: " + ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Lấy bảng dữ liệu (DataTable) từ SELECT
        /// </summary>
        /// <param name="query">Câu lệnh SQL</param>
        /// <param name="parameters">Danh sách tham số</param>
        /// <returns>DataTable</returns>
        /// <example>
        /// Ví dụ dùng:
        /// DataTable dt = DatabaseConnect.ExecuteTable("SELECT * FROM NguoiDung");
        /// foreach (DataRow row in dt.Rows)
        /// {
        ///     string taiKhoan = row["TaiKhoan"].ToString();
        ///     string matKhau = row["MatKhau"].ToString();
        /// }
        /// </example>
        public static DataTable ExecuteTable(string query, params SqlParameter[] parameters)
        {
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        if (parameters != null)
                            cmd.Parameters.AddRange(parameters);

                        using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                        {
                            adapter.Fill(dt);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi Database: " + ex.Message);
            }
            return dt;
        }
    }
}
