using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;

namespace restaurantManager.Services
{
    public static class DatabaseConnect
    {
        // ✅ FIX: Sửa tên database từ QuanLyNhaHang → QuanLyNhaHang1111
        private static string connectionString = @"Data Source=CUONG-LE;Initial Catalog=QuanLyNhaHang;Integrated Security=True;";

        /// <summary>
        /// Thực hiện INSERT, UPDATE, DELETE
        /// </summary>
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
                        return cmd.ExecuteNonQuery() > 0;
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
        /// Lấy 1 giá trị đơn (SELECT COUNT, MAX, ...)
        /// </summary>
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
        /// Lấy DataTable từ SELECT
        /// </summary>
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