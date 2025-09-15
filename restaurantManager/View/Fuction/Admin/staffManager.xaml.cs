using System.Windows;
using System.Windows.Controls;

namespace restaurantManager.View.Fuction.Admin
{
    /// <summary>
    /// Interaction logic for staffManager.xaml
    /// </summary>
    public partial class staffManager : UserControl
    {
        public staffManager()
        {
            InitializeComponent();
        }
        public void hello1(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("hello");
        }
    }
}
