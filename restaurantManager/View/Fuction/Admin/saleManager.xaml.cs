using System.Windows.Controls;
using restaurantManager.ViewModels.Admin;

namespace restaurantManager.View.Fuction.Admin
{
    public partial class saleManager : UserControl
    {
        public saleManager()
        {
            InitializeComponent();
            this.DataContext = new SaleManagerViewModel();
        }
    }
}