using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace restaurantManager
{
    /// <summary>
    /// Interaction logic for Admin.xaml
    /// </summary>
    public partial class Admin : Window
    {
        public Admin()
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