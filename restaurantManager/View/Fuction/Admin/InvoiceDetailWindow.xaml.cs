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

            //thêm cột thành tiền nếu chưa có
            if (!details.Columns.Contains("ThanhTien"))
            {
                details.Columns.Add("ThanhTien", typeof(decimal));
            }

            //tính tiền lại từng món và tổng tiền gốc
            decimal tongGocThucTe = 0;

            foreach (DataRow row in details.Rows)
            {
                //lấy số lượng và đơn giá
                decimal sl = row["SoLuong"] != DBNull.Value ? Convert.ToDecimal(row["SoLuong"]) : 0;
                decimal gia = row["DonGia"] != DBNull.Value ? Convert.ToDecimal(row["DonGia"]) : 0;

                //tính từng món
                decimal thanhTien = sl * gia;

                //gán vào bảng hiển thị
                row["ThanhTien"] = thanhTien;

                //cộng vào tổng gốc
                tongGocThucTe += thanhTien;
            }

            //lấy thông tin khuyến mãi nếu có
            decimal giamKm = invoice.Table.Columns.Contains("GiaTriKhuyenMai") && invoice["GiaTriKhuyenMai"] != DBNull.Value
                ? Convert.ToDecimal(invoice["GiaTriKhuyenMai"])
                : 0;

            string tenKm = invoice.Table.Columns.Contains("TenChuongTrinh")
                ? Convert.ToString(invoice["TenChuongTrinh"])
                : "";

            //tính thanh toán cuối
            decimal thanhToanCuoi = tongGocThucTe - giamKm;
            if (thanhToanCuoi < 0) thanhToanCuoi = 0; //không được âm

            //đổ dữ liệu vào thông tin hóa đơn
            txtInfo.Text =
                $"Mã hóa đơn: {invoice["MaDonHang"]}\n" +
                $"Ngày: {invoice["NgayDat"]}\n" +
                $"Trạng thái: {invoice["TrangThai"]}\n" +
                $"Khuyến mãi: {tenKm}";

            //đưa dữ liệu vào lưới
            dgDetails.ItemsSource = details?.DefaultView;

            //hiển thị chi tiết tiền
            txtSubtotal.Text = $"Tổng tiền gốc: {tongGocThucTe:N0} VNĐ";
            txtDiscountOrd.Text = $"Khuyến mãi: {giamKm:N0} VNĐ";
            txtTotal.Text = $"Thanh toán cuối: {thanhToanCuoi:N0} VNĐ";
        }
    }
}
