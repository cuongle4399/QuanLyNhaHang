using System;
using System.Collections.Generic;
using System.Data;
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
    public partial class revenueManager : UserControl
    {
        private ViewModels.Admin.revenueManager viewModel = new ViewModels.Admin.revenueManager();

        public revenueManager()
        {
            InitializeComponent();
            dpFrom.SelectedDate = DateTime.Today;
            dpTo.SelectedDate = DateTime.Today;
        }

        private void btnSearch_Click(object sender, System.Windows.RoutedEventArgs e)
        {

        }

        private void btnAll_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var table = viewModel.GetAllInvoices();
            dgInvoices.ItemsSource = table.DefaultView;

            var summary = viewModel.GetRevenueSummaryAll();
            if (summary != null)
            {
                txtTotalRevenue.Text = string.Format("{0:N0} VNĐ", summary["TongDoanhThu"]);
                txtInvoiceCount.Text = summary["SoHoaDon"].ToString();
            }
            else
            {
                txtTotalRevenue.Text = "0 VNĐ";
                txtInvoiceCount.Text = "0";
            }
        }

        private void dgInvoices_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (dgInvoices.SelectedItem == null) return;

            var rowView = dgInvoices.SelectedItem as DataRowView;
            if (rowView == null) return;

            int maDonHang = Convert.ToInt32(rowView["MaDonHang"]);

            // lấy chi tiết món ăn của đơn  hàng
            var details = viewModel.GetInvoiceDetails(maDonHang);

            // mở cửa sổ hiển thị hóa đơn
            var wnd = new InvoiceDetailWindow(rowView.Row, details);
            wnd.Owner = Window.GetWindow(this);
            wnd.ShowDialog();
        }
    }
}
