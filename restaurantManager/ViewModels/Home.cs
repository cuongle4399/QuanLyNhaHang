using System;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;

namespace restaurantManager.Services
{
    class ScriptHome
    {
        public static void TogglePassword(PasswordBox pwdBox, TextBox txtBox, bool show)
        {
            if (show)
            {
                txtBox.Text = pwdBox.Password;
                pwdBox.Visibility = Visibility.Collapsed;
                txtBox.Visibility = Visibility.Visible;
            }
            else
            {
                pwdBox.Password = txtBox.Text;
                txtBox.Visibility = Visibility.Collapsed;
                pwdBox.Visibility = Visibility.Visible;
            }
        }

        public static void DangNhap(string username, string password, Window currentWindow)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Vui lòng nhập tài khoản và mật khẩu!");
                return;
            }

            string query = "SELECT VaiTro FROM NguoiDung WHERE MaNhanVien=@tk AND MatKhau=@mk";
            object result = DatabaseConnect.ExecuteScalar(query,
                new SqlParameter("@tk", username),
                new SqlParameter("@mk", password));

            if (result != null && result != DBNull.Value)
            {
                int vaitro = Convert.ToInt32(result);
                Models.SessionUser.UserName = username;
                if (vaitro == 1)
                {
                    Admin admin = new Admin();
                    admin.Show();
                }
                else
                {
                    Staff user = new Staff();
                    user.Show();
                }

                currentWindow.Close();
            }
            else
            {
                MessageBox.Show("Sai tài khoản hoặc mật khẩu!");
            }
        }

        public static void DangKy(string username, string password, string confirmPassword)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin!");
                return;
            }
            if (password != confirmPassword)
            {
                MessageBox.Show("Mật khẩu xác nhận không khớp!");
                return;
            }

            string checkQuery = "SELECT COUNT(*) FROM NguoiDung WHERE MaNhanVien=@tk";
            object result = DatabaseConnect.ExecuteScalar(checkQuery, new SqlParameter("@tk", username));
            int exists = (result != null) ? Convert.ToInt32(result) : 0;

            if (exists > 0)
            {
                MessageBox.Show("Tài khoản đã tồn tại!");
                return;
            }

            string insertQuery = "INSERT INTO NguoiDung (MaNhanVien, MatKhau, VaiTro) VALUES (@tk, @mk, 0)";
            bool success = DatabaseConnect.ExecuteNonQuery(insertQuery,
                new SqlParameter("@tk", username),
                new SqlParameter("@mk", password));

            if (success)
                MessageBox.Show("Đăng ký thành công!");
            else
                MessageBox.Show("Đăng ký thất bại!");
        }

    }
}
