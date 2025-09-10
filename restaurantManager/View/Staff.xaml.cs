using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace restaurantManager
{
    /// <summary>
    /// Interaction logic for Staff.xaml
    /// </summary>
    public partial class Staff : Window
    {
        public Staff()
        {
            InitializeComponent();
            this.Title += $" - Người dùng: {Models.SessionUser.UserName}";
        }
        private void LogoutTab_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            ViewModels.Logout.logout(this);
        }
    }
}
