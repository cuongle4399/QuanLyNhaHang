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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace restaurantManager.View.Fuction.Admin
{
    /// <summary>
    /// Interaction logic for revenueManager.xaml
    /// </summary>
    public partial class revenueManager : UserControl
    {
        private ViewModels.Admin.revenueManager vm;

        public revenueManager()
        {
            InitializeComponent();
            vm = new ViewModels.Admin.revenueManager();
            this.DataContext = vm;

        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            var fromDate = dpFrom.SelectedDate ?? DateTime.Today;
            var toDate = dpTo.SelectedDate ?? DateTime.Today;

            if (fromDate > toDate)
            {
                MessageBox.Show("khoảng ngày không hợp lệ",
                                "sai khoảng", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            vm.Search(fromDate, toDate);
            Refresh();
        }

        private void btnAll_Click(object sender, RoutedEventArgs e)
        {
            vm.LoadAll();
            Refresh();
        }

        private void Refresh()
        {
            dgInvoices.ItemsSource = vm.Invoices;
            txtTotalRevenue.Text = vm.TotalRevenueText;
            txtInvoiceCount.Text = vm.InvoiceCountText;
        }
    }
}