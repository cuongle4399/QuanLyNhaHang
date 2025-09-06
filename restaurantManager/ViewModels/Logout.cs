using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace restaurantManager.ViewModels
{
    internal class Logout
    {
        public static void logout(Window currentWindow)
        {
            if (MessageBox.Show("Bạn có chắc chắn muốn đăng xuất?", "Xác nhận", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                Home home = new Home();
                home.Show();
                currentWindow.Close();
            }
        }
    }
}
