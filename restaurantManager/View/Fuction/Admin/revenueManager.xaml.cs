using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Data;

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
            // Gán ItemsSource lần đầu cho DataGrid
            dgInvoices.ItemsSource = vm.Invoices;
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            var fromDate = dpFrom.SelectedDate ?? DateTime.Today.AddDays(-30); // Giả sử mặc định 30 ngày
            var toDate = dpTo.SelectedDate ?? DateTime.Today;

            if (fromDate > toDate)
            {
                MessageBox.Show("Khoảng ngày không hợp lệ. Ngày bắt đầu phải trước hoặc bằng Ngày kết thúc.",
                                "Lỗi nhập liệu", MessageBoxButton.OK, MessageBoxImage.Warning);
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
            // Do DataView là một kiểu dữ liệu cần được gán lại hoặc làm mới
            // khi DataContext thay đổi nguồn dữ liệu, ta cần gán lại ItemsSource.
            dgInvoices.ItemsSource = vm.Invoices;

            // Đã loại bỏ các dòng gán TextBlock thủ công vì đã sử dụng Data Binding trong XAML.
            // TextBlock sẽ tự động cập nhật khi các Property trong ViewModel thay đổi.
        }

        private void dgInvoices_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (dgInvoices.SelectedItem is DataRowView drv)
            {
                // Lấy dữ liệu chi tiết từ ViewModel
                var (invoiceRow, details) = vm.GetInvoiceDataForDetail(drv);

                if (invoiceRow != null && details != null)
                {
                    // Giả sử InvoiceDetailWindow tồn tại, bạn có thể uncomment code này để mở cửa sổ chi tiết:
                    // var win = new InvoiceDetailWindow(invoiceRow, details);
                    // win.ShowDialog();

                    // Hiện tại chỉ hiển thị MessageBox mô phỏng để tránh lỗi thiếu class InvoiceDetailWindow
                    MessageBox.Show(
                        $"Chi tiết hóa đơn: {invoiceRow["MaDonHang"]}\n" +
                        $"Ngày: {invoiceRow["NgayDat"]:dd/MM/yyyy HH:mm}\n" +
                        $"Tổng tiền gốc: {invoiceRow["TongTienGoc"]:N0} VNĐ\n" +
                        $"Giảm giá: {invoiceRow["GiaTriKhuyenMai"]:N0} VNĐ\n" +
                        $"Thanh toán cuối: {invoiceRow["ThanhToanCuoi"]:N0} VNĐ\n" +
                        $"Nhân viên: {invoiceRow["TenNhanVien"]}",
                        "Xem Chi Tiết Hóa Đơn", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }
    }
}