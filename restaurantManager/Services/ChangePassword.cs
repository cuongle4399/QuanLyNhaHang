using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using restaurantManager.Models;

namespace restaurantManager.Services
{
    public class ChangePassword
    {
        public static string? LayMatKhauHienTai() { 
            var ob = DatabaseConnect.ExecuteScalar("SELECT MatKhau FROM NguoiDung WHERE MaNhanVien=@mnv", 
                                    new SqlParameter("@mnv", SessionUser.UserName)); 
            return ob as string; 
        }
        public static bool CapNhatMatKhau(string matKhauCapNhat) { 
            return DatabaseConnect.ExecuteNonQuery("UPDATE NguoiDung SET MatKhau=@mk WHERE MaNhanVien=@mnv",
                        new SqlParameter("@mnv", SessionUser.UserName), new SqlParameter("@mk", matKhauCapNhat)); 
        }
    }
}
