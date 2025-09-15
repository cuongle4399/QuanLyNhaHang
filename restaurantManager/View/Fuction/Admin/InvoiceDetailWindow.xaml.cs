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
using System.Windows.Shapes;

namespace restaurantManager.View.Fuction.Admin
{
    public partial class InvoiceDetailWindow : Window
    {
        public InvoiceDetailWindow(DataRow invoice, DataTable details)
        {
            InitializeComponent();

            string tenKm = invoice.Table.Columns.Contains("TenChuongTrinh")
                ? Convert.ToString(invoice["TenChuongTrinh"]) : "";
            decimal giamKm = 0;
            if (invoice.Table.Columns.Contains("GiaTriKhuyenMai"))
                decimal.TryParse(Convert.ToString(invoice["GiaTriKhuyenMai"]), out giamKm);

            txtInfo.Text =
                $"Mã hóa đơn: {invoice["MaDonHang"]}\n" +
                $"Ngày: {invoice["NgayDat"]}\n" +
                $"Trạng thái: {invoice["TrangThai"]}\n" +
                $"Khuyến mãi: {tenKm} ({giamKm:N0})";
            dgDetails.ItemsSource = details.DefaultView;

            // tổng thanh toán
            decimal tongCacMon = 0;
            foreach (DataRow r in details.Rows)
                tongCacMon += Convert.ToDecimal(r["ThanhTien"]);

            decimal thanhToan = Math.Max(tongCacMon - giamKm, 0);
            txtSubtotal.Text = $"Tổng các món: {tongCacMon:N0} VNĐ";
            txtDiscountOrd.Text = $"Giảm khuyến mãi: {giamKm:N0} VNĐ";
            txtTotal.Text = $"TỔNG THANH TOÁN: {thanhToan:N0} VNĐ";
        }

    }
}
