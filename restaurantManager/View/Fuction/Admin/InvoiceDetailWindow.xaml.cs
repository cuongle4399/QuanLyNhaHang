using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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
using restaurantManager.Services;

namespace restaurantManager.View.Fuction.Admin
{
    public partial class InvoiceDetailWindow : Window
    {
        public InvoiceDetailWindow(DataRow invoice, DataTable details)
        {
            InitializeComponent();

            //lấy trực tiếp các cột từ DonHang
            decimal tongGoc = invoice.Table.Columns.Contains("TongTienGoc") && invoice["TongTienGoc"] != DBNull.Value
                ? Convert.ToDecimal(invoice["TongTienGoc"])
                : 0;

            decimal giamKm = invoice.Table.Columns.Contains("GiaTriKhuyenMai") && invoice["GiaTriKhuyenMai"] != DBNull.Value
                ? Convert.ToDecimal(invoice["GiaTriKhuyenMai"])
                : 0;

            decimal thanhToan = invoice.Table.Columns.Contains("ThanhToanCuoi") && invoice["ThanhToanCuoi"] != DBNull.Value
                ? Convert.ToDecimal(invoice["ThanhToanCuoi"])
                : 0;

            string tenKm = invoice.Table.Columns.Contains("TenChuongTrinh")
                ? Convert.ToString(invoice["TenChuongTrinh"])
                : "";

            // hiển thị thông tin
            txtInfo.Text =
                $"Mã hóa đơn: {invoice["MaDonHang"]}\n" +
                $"Ngày: {invoice["NgayDat"]}\n" +
                $"Trạng thái: {invoice["TrangThai"]}\n" +
                $"Khuyến mãi: {tenKm}";

            //binding chi tiết món ăn
            dgDetails.ItemsSource = details?.DefaultView;

            //hiển thị tổng tiền
            txtSubtotal.Text = $"Tổng tiền gốc: {tongGoc:N0} VNĐ";
            txtDiscountOrd.Text = $"Khuyến mãi: {giamKm:N0} VNĐ";
            txtTotal.Text = $"Thanh toán cuối: {thanhToan:N0} VNĐ";
        }
    }
}
